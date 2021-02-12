using System.Collections;
using System.Collections.Generic; 

/// <summary>
/// シーンタイプを定数で管理するクラス
/// ※Build SettingsのScenes In Buildに対象シーンをすべて追加してから使用してください
/// </summary>
public static class SceneName
{
	public enum Type
	{
		None,
		Base,
		Pazzle,
	}

	public static readonly Dictionary<Type, string> NameList = new Dictionary<Type, string>()
	{
		{ Type.Base, "BaseScene" },
		{ Type.Pazzle, "PazzleScene" },
	};
}
