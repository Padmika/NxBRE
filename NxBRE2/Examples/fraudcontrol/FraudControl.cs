namespace org.nxbre.examples
{
	using System;
	using System.Collections;
	
	using net.ideaity.util.events;
	
	using org.nxbre.ie;
	using org.nxbre.ie.adapters;
	using org.nxbre.ie.core;
	
	public class FraudControl {
		private string ruleBaseFile;
		private int nbDecaCustomers;
		
		public static int LOG_LEVEL = LogEventImpl.INFO;
		
		private void HandleLogEvent(object obj, ILogEvent aLog)
		{
			if (aLog.Priority >= LOG_LEVEL)
				Console.WriteLine("[" + aLog.Priority + "] " + aLog.Message);
		}	

		public void PerformProcess(IBinder binder)	{
			// generate dummy business objects
			Hashtable businessObjects = DummyData.GetInstance().GetBusinessObjects(nbDecaCustomers);


			// instantiate an inference engine, bind my data and process the rules
			IInferenceEngine ie = new IEImpl(binder);
			ie.LogHandlers += new DispatchLog(HandleLogEvent);
			ie.LoadRuleBase(new RuleML08DatalogAdapter(ruleBaseFile, System.IO.FileAccess.Read));		
			ie.Process(businessObjects);
			
			// processing is done, let's analyze the results
			QueryResultSet qrs = ie.RunQuery("Fraudulent Customers");
			Console.WriteLine("\nDetected {0} fraudulent customers.", qrs.Count);
			if (qrs.Count != 2 * nbDecaCustomers)
				Console.WriteLine("\nError! " + 2* nbDecaCustomers + " was expected.");
			
			// check if the customer objects have been flagged correctly
			int flaggedCount = 0;
			foreach(Customer customer in (ArrayList)businessObjects["CUSTOMERS"])
				if (customer.Fraudulent)
					flaggedCount++;
			
			if (flaggedCount != 2 * nbDecaCustomers)
				throw new Exception("\nError! " + 2* nbDecaCustomers + " flagged Customer objects were expected.\n");
			else
				Console.WriteLine("\nCustomer objects were correctly flagged\n");
		}
		
		/// <summary>
		/// Instantiates a FraudControl system.
		/// </summary>
		/// <param name="ruleBaseFile"></param>
		public FraudControl(int nbDecaCustomers, string ruleBaseFile)
		{
			this.nbDecaCustomers = nbDecaCustomers;
			this.ruleBaseFile = ruleBaseFile;
		}
		
		/// <summary>
		/// Starts the FraudControl system.
		/// </summary>
		/// <param name="args">
		/// args[0] the number of tens of customers to create
		/// args[1] the full path of fraudcontrol.ruleml
		/// </param>
		public static void Main(string[] args) {
			FraudControl fc = new FraudControl(Int32.Parse(args[0]), args[1]);
			
			if (args.Length == 3) LOG_LEVEL = Int32.Parse(args[2]);

			/// Demonstrates how to use a Custom Binder Class
			Console.WriteLine("\n\n************ Using Custom Binder Class ************\n");
			fc.PerformProcess(new CustomBinder());
					
			/// Demonstrates how to use a Custom Compiled Binder
			Console.WriteLine("\n\n************ Using Compiled Custom Binder ************\n");
			fc.PerformProcess(CSharpBinderFactory.LoadFromFile("org.nxbre.examples.CompiledCustomBinder",
			                                            args[1] + ".ccb"));
					
			/// Demonstrates how to use the Flow Engine Binder
			Console.WriteLine("************ Using Flow Engine Binder ************\n");
			fc.PerformProcess(new FlowEngineBinder(args[1] + ".xbre", BindingTypes.BeforeAfter));
		}
	
	}
		
}
