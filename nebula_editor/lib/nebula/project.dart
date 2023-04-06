import 'package:nebula_editor/nebula.dart';

typedef NbAssetBundle = Map<String, NbAssetGroup>;
typedef NbAssetGroup = Map<String, NbAsset>;

class NbProject {
  final String path;
  final String name;
  final String bundleId;
  NbAssetBundle _assets;
  NbAssetBundle _importedAssets;

  NbProject._(this.path, this.name, this.bundleId, this._assets, this._importedAssets);

  NbProject._deserialize(this.path, Map<String, dynamic> json)
      : name = json['name'] as String,
        bundleId = json['bundleId'] as String,
        _assets = (json['assets'] as Map<String, Map<String, Map<String, dynamic>>>?)?.map((groupKey, group) {
              return MapEntry(groupKey, group.map((key, value) {
                return MapEntry(key, NbAsset.from(groupKey, key, value));
              }));
            }) ??
            {},
        _importedAssets = (json['importedAssets'] as Map<String, Map<String, Map<String, dynamic>>>?)?.map((groupKey, group) {
              return MapEntry(groupKey, group.map((key, value) {
                return MapEntry(key, NbAsset.from(groupKey, key, value));
              }));
            }) ??
            {};

  NbAssetBundle get assets => Map.unmodifiable(_assets);
  NbAssetBundle get importedAssets => Map.unmodifiable(_importedAssets);
  NbAssetBundle get allAssets => Map.unmodifiable({..._assets, ..._importedAssets});

  static Future<NbProject> loadProject(String path) async {
    return NbProject._deserialize(path, await Nebula.peer.sendRequest('LoadProject', {'path': path}));
  }

  static Future<NbProject> createAndLoadProject(String path, String name, String bundleId) async {
    return NbProject._deserialize(path, await Nebula.peer.sendRequest('CreateAndLoadProject', {'path': path, 'name': name, 'bundleId': bundleId}));
  }

  static Future<bool> unloadProject() async {
    return await Nebula.peer.sendRequest('UnloadProject');
  }

  Future<void> save() async {
    return await Nebula.peer.sendRequest('SaveProject');
  }
}
