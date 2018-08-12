using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

public static class JsonHelper
{

	private enum ObjectType
	{
		OBJECT,
		ARRAY
	}

	public static JObject Unflatten(IDictionary<string, string> keyValues)
	{
		JContainer result = null;
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
		//build from leaf to root
		foreach (var pathSegment in pathSegments.Reverse())
		{
			var type = GetJsonType(pathSegment);
			switch (type)
			{
				case ObjectType.OBJECT:
					var obj = new JObject();
					if (null == lastItem)
					{
						obj.Add(pathSegment, value);
					}
					else
					{
						obj.Add(pathSegment, lastItem);
					}
					lastItem = obj;
					break;
				case ObjectType.ARRAY:
					var array = new JArray();
					var index = GetArrayIndex(pathSegment);
					array = FillEmpty(array, index);
					if (lastItem == null)
					{
						array[index] = value;
					}
					else
					{
						array[index] = lastItem;
					}
					lastItem = array;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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

	private static ObjectType GetJsonType(string pathSegment)
	{
		return int.TryParse(pathSegment, out int x) ? ObjectType.ARRAY : ObjectType.OBJECT;
	}

	private static int GetArrayIndex(string pathSegment)
	{
		if (int.TryParse(pathSegment, out int result))
		{
			return result;
		}
		throw new Exception("Unable to parse array index: " + pathSegment);
	}

}