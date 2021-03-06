namespace org.nxbre
{
	using System.Collections;
	
	using net.ideaity.util.events;
	
	using org.nxbre.ie;
	using org.nxbre.ie.adapters;
	using org.nxbre.ie.core;
	using org.nxbre.ie.rule;

	/// <summary>
	/// This interface defines the Inference Engine (IE) of NxBRE.
	/// </summary>
	/// <author>David Dossot</author>
	/// <version>2.5</version>
	public interface IInferenceEngine:ILogDispatcher {
		/// <summary>
		/// The event to subscribe in order to be notified of assertion of facts.
		/// </summary>
		event NewFactEvent NewFactHandler;
		
		/// <summary>
		/// The event to subscribe in order to be notified of retraction of facts.
		/// </summary>
		event NewFactEvent DeleteFactHandler;
		
		/// <summary>
		/// The event to subscribe in order to be notified of modification of facts.
		/// </summary>
		event NewFactEvent ModifyFactHandler;
		
		/// <summary>
		/// The optional business object binder type (null if none).
		/// </summary>
		string BinderType { get; }

		/// <summary>
		/// The direction of the loaded rule base.
		/// </summary>
		string Direction { get; }
		
		/// <summary>
		/// The label of the loaded rule base.
		/// </summary>
		string Label { get; }
		
		/// <summary>
		/// The current type of working memory.
		/// </summary>
		WorkingMemoryTypes WorkingMemoryType { get; }

		/// <summary>
		/// Returns true if the engine is initialized with a valid rulebase.
		/// </summary>
		bool Initialized { get; }
		
		/// <summary>
		/// Loads a rule base. The working memory is reset (all facts are lost).
		/// </summary>
		/// <param name="adapter">The Adapter used to read the rule base.</param>
		/// <remarks>
		/// The adapter will be disposed at the end of the method's execution.
		/// </remarks>
		/// <see cref="org.nxbre.ie.adapters.IRuleBaseAdapter"/>
		void LoadRuleBase(IRuleBaseAdapter adapter);
		
		/// <summary>
		/// Loads a rule base. The working memory is reset (all facts are lost).
		/// </summary>
		/// <param name="adapter">The Adapter used to read the rule base.</param>
		/// <param name="businessObjectsBinder">The business object binder that the engine must use.</param>
		/// <remarks>
		/// The adapter will be disposed at the end of the method's execution.
		/// </remarks>
		/// <see cref="org.nxbre.ie.adapters.IRuleBaseAdapter"/>
		void LoadRuleBase(IRuleBaseAdapter adapter, IBinder businessObjectsBinder);
		
		/// <summary>
		/// Saves the WorkingMemory in a rule base.
		/// </summary>
		/// <param name="adapter">The Adapter used to save the rule base.</param>
		/// <remarks>
		/// The adapter will be disposed at the end of the method's execution.
		/// </remarks>
		/// <see cref="org.nxbre.ie.adapters.IRuleBaseAdapter"/>
		void SaveRuleBase(IRuleBaseAdapter adapter);
		
		/// <summary>
		/// Load facts in the current working memory. Current implications, facts and queries
		/// remain unchanged.
		/// </summary>
		/// <remarks>
		/// The adapter will be disposed at the end of the method's execution.
		/// </remarks>
		/// <param name="adapter">The Adapter used to read the fact base.</param>
		void LoadFacts(IFactBaseAdapter adapter);

		/// <summary>
		/// Save facts of the current working memory. Current implications, facts and queries
		/// remain unchanged.
		/// </summary>
		/// <remarks>
		/// The adapter will be disposed at the end of the method's execution.
		/// </remarks>
		/// <param name="adapter">The Adapter used to save the fact base.</param>
		void SaveFacts(IFactBaseAdapter adapter);
		
		/// <summary>
		/// Sets the WorkingMemory of the engine, either by forking the existing Global memory
		/// to a new Isolated one, or by simply using the Global one.
		/// </summary>
		/// <param name="memoryType">The new type of working memory.</param>
		void NewWorkingMemory(WorkingMemoryTypes memoryType);

		/// <summary>
		/// Makes the current isolated memory the new global memory and sets the working memory
		/// type to global. Throws an exception in the current memory type is not isolated.
		/// </summary>
		void CommitIsolatedMemory();
		
		/// <summary>
		/// Dispose the current isolated memory sets the working memory type to global.
		/// Throws an exception in the current memory type is not isolated.
		/// </summary>
		void DisposeIsolatedMemory();
		
		/// <summary>
		/// Performs all the possible deductions on the current working memory and stops
		/// infering when no new Fact is deducted.
		/// </summary>
		void Process();
		
		/// <summary>
		/// If businessObjects is Null, this method performs the same operation as the parameterless
		/// method ; else uses the binder provided in the constructor to perform fact assertions and
		/// orchestrate the process.
		/// If businessObjects is not Null and no binder has been provided in the constructor, throws
		/// a BREException.
		/// </summary>
		/// <param name="businessObjects">An Hashtable of business objects, or Null.</param>
		void Process(Hashtable businessObjects);
		
		/// <summary>
		/// Gets the number of implications in the current rulebase.
		/// </summary>
		int ImplicationsCount { get; }
		
		/// <summary>
		/// Gets the number of facts in the current working memory.
		/// </summary>
		int FactsCount { get; }
		
		/// <summary>
		/// Gets an enumeration of the facts contained in the working memory.
		/// </summary>
		/// <returns>An IEnumerator on the facts contained in the working memory.</returns>
		/// <remarks>Do not alter the facts from this enumemration: use retract and modify instead.</remarks>
		IEnumerator Facts { get; }
		
		/// <summary>
		/// Returns true if a Fact exists in the current working memory.
		/// </summary>
		/// <param name="fact">The Fact to check existence.</param>
		/// <returns>True if the Fact exists.</returns>
		bool FactExists(Fact fact);
		
		/// <summary>
		/// Returns true if a Fact exists in the current working memory.
		/// </summary>
		/// <param name="factLabel">The label of the Fact to check existence.</param>
		/// <returns>True if the Fact exists.</returns>
		bool FactExists(string factLabel);
		
		/// <summary>
		/// Returns a Fact from its label if it exists, else returns null.
		/// </summary>
		/// <param name="factLabel">The label of the Fact to retrieve.</param>
		/// <returns>The Fact that matches the label if it exists, otherwise null.</returns>
		Fact GetFact(string factLabel);
		
		/// <summary>
		/// Asserts (adds) a Fact in the current working memory.
		/// </summary>
		/// <param name="fact">The Fact to assert.</param>
		/// <returns>True if the Fact was added to the Fact Base, i.e. if it was really new!</returns>
		bool Assert(Fact fact);
				
		/// <summary>
		/// Retracts (removes) a Fact from the current working memory.
		/// </summary>
		/// <param name="factLabel">The label of the Fact to retract.</param>
		/// <returns>True if the Fact has been retracted from the FactBase, otherwise False.</returns>
		bool Retract(string factLabel);
		
		/// <summary>
		/// Retracts (removes) a Fact from the current working memory.
		/// </summary>
		/// <param name="fact">The Fact to retract.</param>
		/// <returns>True if the Fact has been retracted from the FactBase, otherwise False.</returns>
		bool Retract(Fact fact);
		
		/// <summary>
		/// Modify a Fact by Retracting it and Asserting the replacement one.
		/// If the new Fact has no label (null or Empty), then the Label of the existing fact is kept.
		/// </summary>
		/// <param name="currentFact">The Fact to modify.</param>
		/// <param name="newFact">The Fact to modify to.</param>
		/// <returns>True if <term>currentFact</term> has been retracted from the FactBase, otherwise False ; this whether <term>newFact</term> already exists in the factbase, or not.</returns>
		bool Modify(Fact currentFact, Fact newFact);
		
		/// <summary>
		/// Modify a Fact by Retracting it and Asserting the replacement one.
		/// If the new Fact has no label (null or Empty), then the Label of the existing fact is kept.
		/// </summary>
		/// <param name="currentFactLabel">The label of the Fact to modify.</param>
		/// <param name="newFact">The Fact to modify to.</param>
		/// <returns>True if <term>currentFact</term> has been retracted from the FactBase, otherwise False ; this whether <term>newFact</term> already exists in the factbase, or not.</returns>
		bool Modify(string currentFactLabel, Fact newFact);

		/// <summary>
		/// Gets the number of queries in the current rulebase.
		/// </summary>
		int QueriesCount { get; }
		
		/// <summary>
		/// Gets the labels of the queries in the current rulebase.
		/// </summary>
		string[] QueryLabels { get; }
		
		/// <summary>
		/// Runs a new Query in the current working memory.
		/// </summary>
		/// <remarks>
		/// For performance reasons, it is recommended to declare all queries in the rule base
		/// and to use RunQuery(queryLabel)
		/// </remarks>
		/// <param name="query">The new Query to run.</param>
		/// <returns>A QueryResultSet containing the results found.</returns>
		/// <see cref="org.nxbre.ie.rule.QueryResultSet"/>
		QueryResultSet RunQuery(Query query);
		
		/// <summary>
		/// Runs a Query in the current working memory.
		/// </summary>
		/// <param name="queryIndex">The query base index of the Query to run.</param>
		/// <returns>A QueryResultSet containing the results found.</returns>
		/// <see cref="org.nxbre.ie.rule.QueryResultSet"/>
		/// <remarks>It is recommanded to use labelled queries.</remarks>
		QueryResultSet RunQuery(int queryIndex);
		
		/// <summary>
		/// Runs a Query in the current working memory.
		/// </summary>
		/// <param name="queryLabel">The label of the Query to run.</param>
		/// <returns>A QueryResultSet containing the results found.</returns>
		/// <see cref="org.nxbre.ie.rule.QueryResultSet"/>
		QueryResultSet RunQuery(string queryLabel);
	}
}
