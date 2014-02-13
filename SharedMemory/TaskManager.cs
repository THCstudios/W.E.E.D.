using System;
using System.Collections.Generic;

namespace SharedMemory
{
	public class TaskManager
	{
		private SortedDictionary<long, Target> targets = new SortedDictionary<long, Target>();
		private Dictionary<Target, TargetState> states = new Dictionary<Target, TargetState>();

		public void pushTarget (long id, Target target)
		{
			if (targets.ContainsKey (id)) {
				throw new Exception("Target already taken");
			}
			targets.Add(id, target);
			states.Add(target, TargetState.WAIT);
		}

		public void runNextTarget ()
		{
			if (states [targets [0]] == TargetState.WAIT) {
				states[targets[0]] = runTarget(targets[0]);
			} else {
				foreach(Target t in targets.Values) {
					if (states[t] == TargetState.WAIT) {
						bool ok = true;
						foreach(Target pre in t.predesecors) {
							if(!targets.ContainsKey(pre.Id) || states[pre] == TargetState.FAIL) {
								states[t] = TargetState.FAIL;
								ok = false;
							} else if (states[pre] == TargetState.WAIT) {
								ok = false;
								break;
							}
						}
						if(ok) {
							TargetState state = runTarget(t);
							states[t] = state;
							return;
						}
					}
				}
			}

		}

		public void RunAll ()
		{
			while (RemainingTasks > 0) {
				runNextTarget();
			}
			Console.WriteLine(StartupInformation);
		}

		private TargetState runTarget(Target t) {
			Console.WriteLine("       Starting Task " + t.Name + " (" + t.Id + ")");
			TargetState state = t.run();
			Console.WriteLine("[" + state + "] Finished Target " + t.Name + " (" + t.Id + ")");
			return state;
		}

		int RemainingTasks {
			get {
				int tasks = 0;
				foreach(Target t in targets.Values) {
					if(states[t] == TargetState.WAIT) {
						tasks++;
					}
				}
				return tasks;
			}
		}

		String StartupInformation {
			get {
				int wait = 0;
				int done = 0;
				int fail = 0;
				foreach(Target t in targets.Values) {
					if(states[t] == TargetState.WAIT) {
						wait++;
					} else if (states[t] == TargetState.DONE) {
						done++;
					} else if (states[t] == TargetState.FAIL) {
						fail++;
					}
				}
				return "DONE: " + done + " WAIT: " + wait + " FAIL: " + fail;
			}
		}

	}
}

