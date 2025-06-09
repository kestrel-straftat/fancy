using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fancy.Resources;

public static class Assets
{
    private static AssetBundle m_bundle;
    
    internal static T Load<T>(string assetName) where T : Object {
        return m_bundle.LoadAsset<T>(assetName);
    }

    internal static void LoadBundle() {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Fancy.Fancy.Resources.Bundles.fancy");
        m_bundle = AssetBundle.LoadFromStream(stream);
    }
}