﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using Helper;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public abstract class AssetTestBase
	{
		protected const String ExamplePath = "Assets/Examples/";

		private readonly TestAssets m_TestAssets = new();

		public AssetTestBase() => AssetDB.CreateFolder(ExamplePath);

		[TearDown] public void TearDown() => Assert.DoesNotThrow(m_TestAssets.Dispose);

		protected Object DeleteAfterTest(Object asset)
		{
			m_TestAssets.Add(asset);
			return asset;
		}
	}
}
