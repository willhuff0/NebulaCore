import 'package:nebula_editor/nebula.dart';

abstract class NbAsset {
  final String guid;

  NbAsset.deserialize(this.guid, Map<String, dynamic> json);

  static NbAsset from(String group, String guid, Map<String, dynamic> json) {
    return nbTypeMap[group]!(guid, json);
  }
}

const nbTypeMap = <String, NbAsset Function(String guid, Map<String, dynamic> json)>{
  'texture': NbTextureAsset.deserialize,
};

abstract class NbFileAsset extends NbAsset {
  final String path;

  NbFileAsset.deserialize(super.guid, super.json)
      : path = json['path'],
        super.deserialize();
}

class NbTextureAsset extends NbFileAsset {
  NbTextureAsset.deserialize(super.guid, super.json) : super.deserialize();
}
