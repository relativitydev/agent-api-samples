using AgentUtility.Agents;
using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace AgentUtility
{
	/// <summary>
	///     This is a integration test that shows how to use the agent utility located in AgentHelpers
	/// </summary>
	[TestFixture]
	[Description("Fixture description here")]
	public class TestAgent
	{
		private IRSAPIClient _client;
		private int _workspaceId;
		private IDBContext dbContext;
		private IServicesMgr servicesManager;
		public TestHelper Helper;
		public int AgentServerID = 0;
		public int ManagerAgentTypeID = 0;
		public int ManagerAgentID = 0;
		public int WorkerAgentTypeID = 0;
		public int WorkerAgentID = 0;
		public string agentName = "Solution Snapshot - Manager";
		public AgentHelpers agentHelper;


		[TestFixtureSetUp]
		public void Execute_TestFixtureSetup()
		{
			//Setup for testing	
			TestHelper helper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			servicesManager = helper.GetServicesManager();

			//Agent setup
			agentHelper = new AgentHelpers(helper);
			SetAgentServerAndAgentTypes(agentHelper);
			CreateAgent(agentHelper);
			EnableAgents(agentHelper);

			//create client
			_client = helper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

		}


		[TestFixtureTearDown]
		public void Execute_TestFixtureTeardown()
		{
			DeleteAgents(agentHelper);

		}

		[Test]
		[Description("Test description here")]
		public void Integration_Test_Golden_Flow_Valid()
		{

		}


		public void SetAgentServerAndAgentTypes(AgentHelpers agentHelper)
		{
			var agentTypeResponse = agentHelper.ReadAgentTypeByName(agentName);
			ManagerAgentTypeID = agentTypeResponse.ArtifactID;

			var agentServerResponse = agentHelper.ReadAgentServerByAgentType(ManagerAgentTypeID);
			AgentServerID = agentServerResponse.ArtifactID;
		}

		public void CreateAgent(AgentHelpers agentHelper)
		{
			ManagerAgentID = agentHelper.Create(ManagerAgentTypeID, AgentServerID, enabled: false);
		}


		public void EnableAgents(AgentHelpers agentHelper)
		{
			agentHelper.Update(ManagerAgentID, ManagerAgentTypeID, AgentServerID, enabled: true);
		}

		public void DeleteAgents(AgentHelpers agentHelper)
		{
			agentHelper.Delete(ManagerAgentID);
		}

	}
}