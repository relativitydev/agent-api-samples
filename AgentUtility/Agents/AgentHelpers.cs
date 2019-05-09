using Relativity.API;
using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgentUtility.Agents
{
	public class AgentHelpers
	{
		private TestHelper _helper;
		private IHelper helper;

		public AgentHelpers(TestHelper helper)
		{
			_helper = helper;
		}

		public AgentHelpers(IHelper helper)
		{
			this.helper = helper;
		}

		/// <summary>
		/// Returns a list of all available agent types.
		/// </summary>
		/// <returns></returns>
		public AgentTypeResponse ReadAgentTypeByName(string name)
		{
			bool success = false;

			List<AgentTypeResponse> agentTypes = null;
			using (var agentManager = _helper.GetServicesManager()
				.CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				agentTypes = agentManager.GetAgentTypesAsync(-1).Result;


				var selectedAgentType = agentTypes.FirstOrDefault(a => a.Name == name);

				if (selectedAgentType == null)
				{
					throw new Exception($"No agent by name: {name} was found.");
				}

				return selectedAgentType;
			}
		}

		/// <summary>
		/// Returns the first server compatible with the agent type.
		/// </summary>
		/// <returns></returns>
		public AgentServerResponse ReadAgentServerByAgentType(int agentTypeID)
		{
			List<AgentServerResponse> resourceServers = null;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				resourceServers = agentManager.GetAvailableAgentServersAsync(-1, agentTypeID).Result;
			}
			return resourceServers.FirstOrDefault();
		}

		/// <summary>
		/// Creates an agent
		/// </summary>
		/// <param name="agentType"></param>
		/// <param name="agentServerRef"></param>
		/// <returns></returns>
		public int Create(int agentTypeID, int agentServerID, bool enabled = false)
		{
			int agentID = -1;

			var agentType = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentTypeID });
			var agentServer = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentServerID });
			var agentToCreate = new AgentRequest()
			{
				AgentType = agentType,
				Enabled = enabled,
				Interval = 5,
				AgentServer = agentServer,
				Keywords = "Integration Test Agent",
				LoggingLevel = 1
			};

			using (var agentManager = _helper.GetServicesManager().CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				// Confirms an agent may be created
				List<AgentInstanceLimitResult> results = agentManager.ValidateCreateInstanceLimitAsync(-1, agentToCreate, 1).Result;
				foreach (var result in results)
				{
					Console.WriteLine(result.Limit.ToString());
				}

				agentID = agentManager.CreateAsync(-1, agentToCreate).Result;
			}
			return agentID;
		}

		public void Update(int agentID, int agentTypeID, int agentServerID, bool enabled = true)
		{
			var agentType = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentTypeID });
			var agentServer = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = agentServerID });

			var request = new AgentRequest
			{
				Enabled = enabled,
				AgentServer = agentServer,
				AgentType = agentType,
				LoggingLevel = 1,
				Interval = 5
			};

			using (var agentManager = _helper.GetServicesManager().CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				agentManager.UpdateAsync(-1, agentID, request).Wait();
			}
		}

		/// <summary>
		/// Deletes an agent by ID.
		/// </summary>
		/// <param name="agentID"></param>
		public void Delete(int agentID)
		{
			using (var agentManager = _helper.GetServicesManager().CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				// Confirms an agent may be deleted
				List<AgentInstanceLimitResult> results = agentManager.ValidateDeleteInstanceLimitAsync(-1, agentID).Result;
				foreach (var result in results)
				{
					Console.WriteLine(result.Limit.ToString());
				}

				agentManager.DeleteAsync(-1, agentID).Wait();
			}
		}
	}

}

