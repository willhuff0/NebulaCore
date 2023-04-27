import 'package:nebula_editor/nebula.dart';

class NbSceneAsset extends NbAsset {
  final List<NbNode> _roots;

  List<NbNode> get roots => List.unmodifiable(_roots);

  NbSceneAsset.deserialize(super.guid, super.json)
      : _roots = (json['roots'] as List<Map<String, dynamic>>?)?.map((node) => NbNode.deserialize(node)).toList() ?? [],
        super.deserialize();

  Future<NbScene> load() async {
    return NbScene._(guid, name, await Nebula.peer.sendRequest('LoadScene', {'guid': guid}));
  }

  @override
  String get groupKey => 'scenes';
}

class NbScene {
  final String guid;
  final String name;
  final Map<String, Map<String, dynamic>> _runtimeAssets;

  NbScene._(this.guid, this.name, this._runtimeAssets);

  Map<String, Map<String, dynamic>> get runtimeAssets => Map.unmodifiable(_runtimeAssets);

  Future<void> unload() async {
    await Nebula.peer.sendRequest('UnloadScene', {'guid': guid});
  }

  Future<void> save() async {
    await Nebula.peer.sendRequest('SaveScene', {'guid': guid});
  }
}
