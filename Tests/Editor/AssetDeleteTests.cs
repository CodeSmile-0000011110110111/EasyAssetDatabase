﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;

public class AssetDeleteTests : AssetTestBase
{
	[Test] public void Delete_ExistingAssetObject_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Delete(asset);

		Assert.False(AssetHelper.FileExists(asset));
	}

	[Test] public void Delete_ExistingAssetPath_FileDeleted()
	{
		var asset = CreateTestAsset(TestAssetPath);
		Assert.True(AssetHelper.FileExists(asset));

		Asset.Delete(TestAssetPath);

		Assert.False(AssetHelper.FileExists(asset));
	}
}