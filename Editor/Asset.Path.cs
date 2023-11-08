﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.IO;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     <p>
		///         Represents a relative path to an asset file or folder under either 'Assets' or 'Packages'.
		///         Instances can be initialized with a relative or full (absolute) path, internally it will be converted
		///         to a relative path. Use the FullPath property to get the full (absolute) path.
		///     </p>
		///     <p>
		///         All path separators are converted to forward slashes for compatibility with Windows, Mac, Linux.
		///         Leading and trailing path separators are trimmed: "\Assets\folder\" => "Assets/folder"
		///     </p>
		///     <p>
		///         Instances are implicitly and explicitly convertible to/from string:
		///         <example>string strPath = (string)new Asset.Path("Assets/MyFolder/My.file");</example>
		///         <example>Asset.Path assetPath = (Asset.Path)"Assets/MyFolder/My.file";</example>
		///     </p>
		///     <p>
		///         Ideally you should pass in a string and henceforth work with the Asset.Path instance,
		///         since path sanitation occurs every time an Asset.Path instance is created.
		///     </p>
		/// </summary>
		public partial class Path : IEquatable<Path>, IEquatable<String>
		{
			public const String DefaultExtension = "asset";

			private String m_RelativePath = String.Empty;

			/// <summary>
			///     Returns the extension of the file path.
			/// </summary>
			/// <value>The extension with a leading dot (eg '.txt') or an empty string.</value>
			public String Extension => System.IO.Path.GetExtension(m_RelativePath);
			/// <summary>
			///     Returns the file name with extension.
			/// </summary>
			public String FileName => System.IO.Path.GetFileName(m_RelativePath);
			/// <summary>
			///     Returns the file name without extension.
			/// </summary>
			public String FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(m_RelativePath);
			/// <summary>
			///     Returns the directory name.
			/// </summary>
			public String DirectoryName => System.IO.Path.GetDirectoryName(m_RelativePath).ToForwardSlashes();

			/// <summary>
			///     Returns the path to the project's 'Assets' subfolder.
			/// </summary>
			public static String FullAssetsPath => Application.dataPath;
			/// <summary>
			///     Returns the path to the project's 'Packages' subfolder.
			/// </summary>
			public static String FullPackagesPath => $"{FullProjectPath}/Packages";
			/// <summary>
			///     Returns the path to the project's root folder.
			/// </summary>
			public static String FullProjectPath => FullAssetsPath.Substring(0, Application.dataPath.Length - 6);

			/// <summary>
			///     Creates and returns the full path, with forward slashes as separators.
			/// </summary>
			public String FullPath => System.IO.Path.GetFullPath(m_RelativePath).ToForwardSlashes();

			/// <summary>
			///     Returns the path to the file's parent folder, or the path itself if the path points to a folder.
			///     CAUTION: The path must exist! If not, throws an exception.
			/// </summary>
			/// <exception cref="InvalidOperationException">if the path does not exist</exception>
			public Path FolderPath
			{
				get
				{
					// existing directory? return that
					if (Directory.Exists(m_RelativePath))
						return this;

					// existing file? return folder path
					if (File.Exists(m_RelativePath))
						return ToFolderPath();

					throw new InvalidOperationException("unable to determine if file or folder because path" +
					                                    $" '{m_RelativePath}' does not exist");
				}
			}

			/// <summary>
			///     Returns the path to the file's parent folder, or the path itself if the path points to a folder.
			///     If the path does not exist and it ends with an extension (has a dot) then it is assumed a file path,
			///     otherwise a folder path is assumed (Unity does not allow assets without extensions).
			///     CAUTION: This may incorrectly assume a file if the path's last folder contains a dot. In this case
			///     it returns the second to last folder in the path.
			/// </summary>
			public Path FolderPathAssumptive
			{
				get
				{
					// existing directory? return that
					if (Directory.Exists(m_RelativePath))
						return this;

					// existing file? return folder path
					if (File.Exists(m_RelativePath))
						return ToFolderPath();

					// if it has an extension, assume it's a file (could also be a folder but alas ...)
					if (System.IO.Path.HasExtension(m_RelativePath))
						return ToFolderPath();

					return this;
				}
			}

			private static String ToRelative(String fullOrRelativePath)
			{
				if (IsRelative(fullOrRelativePath))
					return fullOrRelativePath.Trim('/');

				var fullPath = fullOrRelativePath;
				ThrowIf.NotAProjectPath(fullPath);
				return MakeRelative(fullPath);
			}

			private static Boolean IsRelative(String path)
			{
				// path must start with "Assets" or "Packages/"
				// it may also be just "Assets" (length == 6), otherwise a path separator must follow: "Assets/.."
				path = path.TrimStart('/').ToLower();
				var startsWithAssets = path.StartsWith("assets");
				var startsWithPackages = path.StartsWith("packages/");
				return startsWithAssets && (path.Length <= 6 || path[6].Equals('/')) || startsWithPackages;
			}

			private static String MakeRelative(String fullOrRelativePath) =>
				fullOrRelativePath.Substring(FullProjectPath.Length).Trim('/');

			/// <summary>
			///     Returns the relative path as string. Same as implicit string conversion.
			/// </summary>
			/// <returns></returns>
			public override String ToString() => m_RelativePath;
		}
	}
}
