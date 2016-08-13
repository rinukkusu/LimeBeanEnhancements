using System;

namespace LimeBeanEnhancements.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class BeanPropertyAttribute : Attribute
	{
		public string Column { get; private set; }
		public int Position { get; private set; }
		public bool SaveToDatabase { get; private set; }

		public BeanPropertyAttribute(string column, int position = 1, bool saveToDatabase = true)
	 {
			Column = column;
			Position = position;
			SaveToDatabase = saveToDatabase;
	 }
	}
}
