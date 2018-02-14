using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using vautointerview.Models;
using System.Linq;
using System.Threading;

namespace vautointerview
{
    class Program
    {
        private static bool VerboseFlag = false;

        /// <summary>
        /// vautointerview - accepts a verbose parameter of true to show execution details
        /// vAuto Programming Challenge - retrieves a datasetID, retrieves all vehicles and dealers for that dataset, and successfully posts to the answer endpoint 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                VerboseFlag = Boolean.Parse(args[0]);
            }

            Console.WriteLine($"Starting {DateTime.Now.ToLongTimeString()}");

            var answer = GetDealerVehicles();

            var answerResponse = PostAnswer(answer).GetAwaiter().GetResult();

            if (answerResponse != null)
            {
                Console.WriteLine($"{answerResponse.Message} Answer result Success={answerResponse.Success} Total Milliseconds={answerResponse.TotalMilliseconds}");
            }
            else
            {
                throw new Exception("Answer Response was not received");
            }
            Console.WriteLine();
            Console.WriteLine($"Completed {DateTime.Now.ToLongTimeString()}");
            Console.WriteLine();
            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();
        }

        private static void VerboseWriteMessage(string message)
        {
            if (VerboseFlag)
            {
                Console.WriteLine(message);
            }
        }

        public static AnswerPost GetDealerVehicles()
        {

            // that could be performed concurrently on the ConcurrentDictionary.  However, global
            // operations like resizing the dictionary take longer as the concurrencyLevel rises. 
            // For the purposes of this example, we'll compromise at numCores * 2.
            int concurrencyLevel = Environment.ProcessorCount * 2;
            //Get Dataset id that will be used in subsequent API calls
            var dataSet = GetDataSet().Result;
            if (dataSet != null)
            {
                VerboseWriteMessage($"DataSet Id {dataSet.Id} {DateTime.Now.ToLongTimeString()}");

                var Vehicles = GetVehicles(dataSet.Id, concurrencyLevel);

                if (Vehicles != null && Vehicles.Values != null)
                {
                    var Dealers = GetDealers(
                                    dataSet.Id,
                                    Vehicles.Values.Select(item => item.DealerId).Distinct().ToList(), //unique list of Dealer Ids
                                    concurrencyLevel);
                    if (Dealers != null)
                    {
                        //Collect the list of vehicles per dealer
                        foreach (var dealer in Dealers.Values)
                        {
                            dealer.Vehicles = new List<Vehicle>();
                            dealer.Vehicles.AddRange(Vehicles.Values.Where(item => item.DealerId == dealer.Id).ToList());
                        }

                        //Create a serializable answer that contains all Dealers 
                        //where each Dealer has a nested list of vehicles for that dealer
                        AnswerPost answerPost = new AnswerPost();
                        answerPost.DataSetId = dataSet.Id;
                        answerPost.Dealers = new List<Dealer>();
                        answerPost.Dealers.AddRange(Dealers.Values);

                        return answerPost;
                    }
                    else
                    {
                        throw new Exception($"Unable to retreive a list of dealers for dataset {dataSet.Id}");
                    }

                }
                else
                {
                    throw new Exception($"Unable to retreive a list of vehicles for dataset {dataSet.Id}");
                }
            }
            else
            {
                throw new Exception("Unable to retreive initial DataSet from the api");
            }
        }

        private static ConcurrentDictionary<int, Vehicle> GetVehicles(string datasetId,
                                                                            int concurrencyLevel)
        {
            //Get the list of Vehicle Ids for this DataSet
            var vehicleList = GetVehicleIdList(datasetId).Result;

            //Use a thread safe dictionary to hold a collection of vehicles
            //Will set the ConcurrentDictionary so it does not need to be resized when it is intialized
            var Vehicles = new ConcurrentDictionary<int, Vehicle>(concurrencyLevel, vehicleList.vehicleIds.Count);

            //Set up a pool of threads, one per vehicle, to asynchronously retrieve all vehicle's details
            var tasks = new List<Task>();
            foreach (var vehicleId in vehicleList.vehicleIds)
            {
                tasks.Add(Task.Run(async delegate
                {
                    await GetVehicleDetails(datasetId, vehicleId, Vehicles);
                }));
            }
            //Wait until all threads retrieve vehicle data from the API has completed
            Task.WaitAll(tasks.ToArray());
            if (VerboseFlag)
            {
                VerboseWriteMessage(string.Empty);
                VerboseWriteMessage($"Vehicles {DateTime.Now.ToLongTimeString()}");
                foreach (var vehicle in Vehicles.Values)
                {
                    VerboseWriteMessage(vehicle.ToString());
                }
            }

            return Vehicles;
        }

        private static ConcurrentDictionary<int, Dealer> GetDealers(string datasetId,
                                                                            List<int?> dealerIds,
                                                                            int concurrencyLevel)
        {
            //Use a thread safe dictionary to hold a collection of dealers
            //Will set the ConcurrentDictionary so it does not need to be resized when it is intialized
            var Dealers = new ConcurrentDictionary<int, Dealer>(concurrencyLevel, dealerIds.Count);

            //Set up a pool of threads, one per deaker, to asynchronously retrieve all dealer's details
            var tasks = new List<Task>();
            foreach (var dealerid in dealerIds)
            {
                tasks.Add(Task.Run(async delegate
                {
                    await GetDealer(datasetId, dealerid, Dealers);
                }));

            }
            //Wait until all threads retrieve dealer data from the API has completed
            Task.WaitAll(tasks.ToArray());
            if (VerboseFlag)
            {
                VerboseWriteMessage(string.Empty);
                VerboseWriteMessage($"Dealers {DateTime.Now.ToLongTimeString()}");
                foreach (var dealer in Dealers.Values)
                {
                    VerboseWriteMessage(dealer.ToString());
                }
            }
            return Dealers;
        }

        public static async Task<DataSet> GetDataSet()
        {
            var apiPath = "api/datasetid";
            return await APICall<DataSet>.GetResult(apiPath);
        }

        public static async Task<Vehicles> GetVehicleIdList(string datasetId)
        {

            var apiPath = $"api/{datasetId}/vehicles";
            return await APICall<Vehicles>.GetResult(apiPath);
        }

        public static async Task GetVehicleDetails(string datasetId,
                                                   int? vehicleId,
                                                   ConcurrentDictionary<int, Vehicle> vehicles)
        {

            if (!String.IsNullOrEmpty(datasetId) && vehicleId != null)
            {
                VerboseWriteMessage($"Thread id {Thread.CurrentThread.ManagedThreadId} {DateTime.Now.ToLongTimeString()}");
                var apiPath = $"api/{datasetId}/vehicles/{vehicleId}";
                var vehicle = await APICall<Vehicle>.GetResult(apiPath);
                vehicles.TryAdd((int)vehicleId, vehicle);
                VerboseWriteMessage($"Thread id {Thread.CurrentThread.ManagedThreadId} Complete {DateTime.Now.ToLongTimeString()}");
            }
            else
            {
                throw new Exception($"GetVehicleDetails requires both a DataSet Id and Vehicle Id");
            }
        }
        public static async Task GetDealer(string datasetId,
                                           int? dealerId,
                                           ConcurrentDictionary<int, Dealer> dealers)
        {

            if (!String.IsNullOrEmpty(datasetId) && dealerId != null)
            {
                VerboseWriteMessage($"Thread id {Thread.CurrentThread.ManagedThreadId}");
                var apiPath = $"api/{datasetId}/dealers/{dealerId}";
                var dealer = await APICall<Dealer>.GetResult(apiPath);
                dealers.TryAdd((int)dealerId, dealer);
                VerboseWriteMessage($"Thread id {Thread.CurrentThread.ManagedThreadId} Complete {DateTime.Now.ToLongTimeString()}");
            }
            else
            {
                throw new Exception($"GetDealer requires both a DataSet Id and Dealer Id");
            }
        }

        public static async Task<AnswerResponse> PostAnswer(AnswerPost answerPost)
        {
            if (answerPost != null)
            {
                var apiPath = $"api/{answerPost.DataSetId}/answer";
                return await APICall<AnswerResponse>.PostAnswer(apiPath, answerPost);
            }
            else
            {
                throw new Exception($"PostAnswer requires an AnswerPost object");
            }

        }
    }
}
