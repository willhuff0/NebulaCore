import 'package:nebula_editor/nebula.dart';

abstract class NbAsset {
  final String name;
  final String guid;

  NbAsset.deserialize(this.guid, Map<String, dynamic> json) : name = json['name'] as String;

  static NbAsset from(String group, String guid, Map<String, dynamic> json) {
    return deserializers[group]!(guid, json);
  }

  static const deserializers = <String, NbAsset Function(String guid, Map<String, dynamic> json)>{
    'scenes': NbSceneAsset.deserialize,
    'textures': NbTextureAsset.deserialize,
  };
}

abstract class NbFileAsset extends NbAsset {
  final String path;

  NbFileAsset.deserialize(super.guid, super.json)
      : path = json['path'],
        super.deserialize();
}

class NbTextureAsset extends NbFileAsset {
  NbTextureAsset.deserialize(super.guid, super.json) : super.deserialize();
}
