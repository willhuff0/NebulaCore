import 'package:nebula_editor/nebula.dart';

class NbProject {
  final String name;
  final String bundleId;
  Map<String, Map<String, NbAsset>> _assets;

  NbProject._(this.name, this.bundleId, this._assets);

  Map<String, Map<String, NbAsset>> get assets => Map.unmodifiable(_assets);

  static Future<NbProject> loadProject(String path) async {
    final json = await Nebula.peer.sendRequest('LoadProject', {'path': path});
    final assets = (json['assets'] as Map<String, Map<String, Map<String, dynamic>>>?)?.map((groupKey, group) {
      return MapEntry(groupKey, group.map((key, value) {
        return MapEntry(key, NbAsset.from(groupKey, key, value));
      }));
    });
    return NbProject._(json['name'], json['bundleId'], assets ?? {});
  }

  static Future<bool> unloadProject() async {
    return await Nebula.peer.sendRequest('UnloadProject');
  }
}
