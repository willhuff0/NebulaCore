import 'package:nebula_editor/nebula.dart';
import 'package:path/path.dart' as p;

class NbTextureAsset extends NbFileAsset {
  NbTextureAsset.create(super.name, super.path) : super.create();

  NbTextureAsset.deserialize(super.guid, super.json) : super.deserialize();

  static Future<NbTextureAsset> import(String path) async {
    return NbTextureAsset.create(p.basename(path), path);
  }

  @override
  String get groupKey => 'textures';
}
