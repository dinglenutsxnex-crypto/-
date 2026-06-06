using System.Collections.Generic;

namespace SF3.VerlePhysics
{
	public class PhysicWorld : LocalWorld
	{
		private int NumberOfIterations = 10;

		private float lastTime;

		private float lastTimeDelta;

		private float gravity;

		private Node[] nodeArray = new Node[0];

		private Edge[] edgeArray = new Edge[0];

		private BoneController[] controllerArray = new BoneController[0];

		private ChainController[] chainArray = new ChainController[0];

		public Node[] getNodesArray
		{
			get
			{
				return nodeArray;
			}
		}

		public PhysicWorld(float nowTime, float inGravity)
		{
			lastTime = nowTime;
			lastTimeDelta = 0f;
			gravity = inGravity;
		}

		public void TimeStep(float nowTime)
		{
			float nowDelta = nowTime - lastTime;
			ElementsTimeStep.physicNodeTimeStep(nodeArray, lastTimeDelta, nowDelta, gravity);
			ElementsTimeStep.jointNodeTimeStep(nodeArray);
			ElementsTimeStep.edgeTimeStep(edgeArray);
			IterativeProcess(NumberOfIterations);
			ElementsTimeStep.jointTimeStep(controllerArray);
			ElementsTimeStep.jointTimeStep(chainArray);
			lastTimeDelta = nowDelta;
			lastTime = nowTime;
		}

		public void IterativeProcess(int numberOfIterations)
		{
			for (int i = 0; i < numberOfIterations; i++)
			{
				Iterate();
			}
		}

		public void Iterate()
		{
			ElementIterator.IterateEdges(edgeArray);
			ElementIterator.IterateNodes(nodeArray);
		}

		public void prepareArrays()
		{
			nodeArray = nodes.ToArray();
			edgeArray = edges.ToArray();
			controllerArray = controllers.ToArray();
			chainArray = chains.ToArray();
		}

		public void ConsumeLocalWorld(LocalWorld toConsume)
		{
			nodes.AddRange(toConsume.getNodes);
			edges.AddRange(toConsume.getEdges);
			controllers.AddRange(toConsume.getControllers);
			chains.AddRange(toConsume.getChains);
			prepareArrays();
		}

		public void ConsumeLocalWorlds(List<LocalWorld> toConsume)
		{
			for (int i = 0; i < toConsume.Count; i++)
			{
				ConsumeLocalWorld(toConsume[i]);
			}
		}
	}
}
