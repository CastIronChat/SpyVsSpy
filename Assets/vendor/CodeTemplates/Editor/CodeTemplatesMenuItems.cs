using UnityEditor;
using UnityEngine;

namespace XORHEAD.CodeTemplates
{
	/// <summary>
	/// Definitions for all the Code templates menu items.
	/// </summary>
	public class CodeTemplatesMenuItems
	{
		/// <summary>
		/// The root path for code templates menu items.
		/// </summary>
		private const string MENU_ITEM_PATH = "Assets/Create/";

		/// <summary>
		/// Menu items priority (so they will be grouped/shown next to existing scripting menu items).
		/// </summary>
		private const int MENU_ITEM_PRIORITY = 70;

		// /// <summary>
		// /// Creates a new C# class.
		// /// </summary>
		// [MenuItem(MENU_ITEM_PATH + "C# Class", false, MENU_ITEM_PRIORITY)]
		// private static void CreateClass()
		// {
		// 	CodeTemplates.CreateFromTemplate(
		// 		"NewClass.cs",
		// 		@"Assets/CodeTemplates/Editor/Templates/ClassTemplate.txt");
		// }

		// /// <summary>
		// /// Creates a new C# interface.
		// /// </summary>
		// [MenuItem(MENU_ITEM_PATH + "C# Interface", false, MENU_ITEM_PRIORITY)]
		// private static void CreateInterface()
		// {
		// 	CodeTemplates.CreateFromTemplate(
		// 		"NewInterface.cs",
		// 		@"Assets/CodeTemplates/Editor/Templates/InterfaceTemplate.txt");
		// }
	}
}
