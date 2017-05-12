using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSourceWithTriggerObject
{
	/// <summary>
	/// Single object instead of firing an event
	/// </summary>
	public class ManualTrigger
	{
		static readonly object loc = new object();
		static ManualTrigger instance;

		public static ManualTrigger Get()
		{
			lock (loc)
			{
				if (instance == null)
					instance = new ManualTrigger();
			}

			return instance;
		}

		private ManualTrigger()
		{
		}

		//Call this method to trigger the event
		public void Trigger(string triggerState)
		{
			Triggered?.Invoke(this, triggerState);
		}

		public event EventHandler<string> Triggered;
	}
}
