using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BaseEventLogger
{
	protected class LoggerData
	{
		public List<JObject> events = new List<JObject>();

		public long eventID;
	}

	protected static LoggerData eventsData = new LoggerData();

	private const string SERIALIZE_TAG = "event_queue";

	public BaseEventLogger()
	{
		Load();
	}

	public void SetEventIDIfHighter(long eventID)
	{
		RemoveEventsLessOrEqual(eventID);
		if (eventID > eventsData.eventID)
		{
			eventsData.eventID = eventID;
		}
	}

	public void AddEvent(string etype, string key, JToken value)
	{
		JObject jObject = new JObject();
		jObject.Add(key, value);
		AddEvent(etype, jObject);
	}

	public virtual void AddEvent(string etype, JObject eventData)
	{
		if (ShouldBeLogged(etype))
		{
			eventsData.eventID++;
			eventData.Add("etype", etype);
			AddGeneralData(eventData);
			eventsData.events.Add(eventData);
			Save();
		}
	}

	protected virtual void AddGeneralData(JObject data)
	{
		data.Add("cid", eventsData.eventID);
	}

	protected virtual bool ShouldBeLogged(string etype)
	{
		return true;
	}

	protected void RemoveEvent(JObject eventToRemove)
	{
		string text = eventToRemove["cid"].ToString();
		for (int num = eventsData.events.Count - 1; num >= 0; num--)
		{
			JObject jObject = eventsData.events[num];
			if (jObject["cid"].ToString(Formatting.None) == text)
			{
				eventsData.events.RemoveAt(num);
				break;
			}
		}
	}

	private void RemoveEventsLessOrEqual(long eventID)
	{
		for (int num = eventsData.events.Count - 1; num >= 0; num--)
		{
			JObject jObject = eventsData.events[num];
			if (long.Parse(jObject["cid"].ToString(Formatting.None)) <= eventID)
			{
				eventsData.events.RemoveAt(num);
				break;
			}
		}
	}

	protected void Save()
	{
		PlayerPrefs.SetString("event_queue", JsonConvert.SerializeObject(eventsData, Formatting.None));
	}

	private void Load()
	{
		if (PlayerPrefs.HasKey("event_queue"))
		{
			eventsData = JsonConvert.DeserializeObject<LoggerData>(PlayerPrefs.GetString("event_queue"));
		}
	}
}
