using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

public static class JsonHelper
{

	private enum JSONType
	{
		OBJECT,
		ARRAY
	}

	public static JObject Unflatten(IDictionary<string, string> keyValues)
	{
		var result = (JContainer)null;
		var setting = new JsonMergeSettings();
		setting.MergeArrayHandling = MergeArrayHandling.Merge;
		foreach (var pathValue in keyValues)
		{
			if (result == null)
			{
				result = UnflatenSingle(pathValue);
			}
			else
			{
				result.Merge(UnflatenSingle(pathValue), setting);
			}
		}
		return result as JObject;
	}

	private static JContainer UnflatenSingle(KeyValuePair<string, string> keyValue)
	{
		var path = keyValue.Key;
		var value = keyValue.Value;
		var pathSegments = SplitPath(path);

		var lastItem = (JContainer)null;
		foreach (var pathSegment in pathSegments.Reverse())
		{
			var type = GetJsonType(pathSegment);
			var obj = new JObject();
			if (null == lastItem)
			{
				obj.Add(pathSegment.ToString(), value);
			}
			else
			{
				obj.Add(pathSegment.ToString(), lastItem);
			}

			lastItem = obj;
		}
		return lastItem;
	}

	public static IList<string> SplitPath(string path)
	{
		var reg = new Regex(@"(?!\.)([^. ^\[\]]+)|(?!\[)(\d+)(?=\])");
		return (from Match match in reg.Matches(path)select match.Value).ToList();
	}

	private static JArray FillEmpty(JArray array, int index)
	{
		for (var i = 0; i <= index; i++)
		{
			array.Add(null);
		}

		return array;
	}

	private static JSONType GetJsonType(string pathSegment)
	{
		var x = 0;
		return int.TryParse(pathSegment, out x) ? JSONType.ARRAY : JSONType.OBJECT;
	}

	private static int GetArrayIndex(string pathSegment)
	{
		var result = 0;
		if (int.TryParse(pathSegment, out result))
		{
			return result;
		}
		throw new Exception("Unable to parse array index: " + pathSegment);
	}
}