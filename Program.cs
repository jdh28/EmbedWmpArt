/*
 * John Hall <john@jdh28.co.uk>
 * Copyright (c) John Hall. All rights reserved.
 * 
 * This program is licensed under the MIT licence, see LICENSE.md.
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using TagLib;

namespace EmbedWmpArt
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length > 1)
			{
				Console.Error.Write("Unexpected arguments");
				return 1;
			}

			try
			{
				var rootDir = args.Length == 1 ? args[0] : Environment.CurrentDirectory;

				foreach (var coverImage in Directory.EnumerateFiles(rootDir, "Folder.jpg", SearchOption.AllDirectories))
					ProcessDir(rootDir, coverImage);

				return 0;
			}
			catch (IOException ioe)
			{
				Console.Error.WriteLine(ioe.Message);
			}
			catch (UnauthorizedAccessException uae)
			{
				Console.Error.WriteLine(uae.Message);
			}
			catch (SecurityException se)
			{
				Console.Error.WriteLine(se.Message);
			}

			return 1;
		}

		private static void ProcessDir(string rootDir, string coverImage)
		{
			var dir = Path.GetDirectoryName(coverImage);
			Picture picture = null;

			foreach (var mp3 in Directory.EnumerateFiles(dir, "*.mp3").Select(f => TagLib.File.Create(f)))
			{
				if (mp3.Tag.Pictures.Length == 0)
				{
					var displayName = mp3.Name.Substring(rootDir.Length + 1);
					Console.Out.WriteLine(displayName);

					if (picture == null)
						picture = new Picture(coverImage);

					mp3.Tag.Pictures = new[] { picture };
					mp3.Save();
				}
			}
		}
	}
}
