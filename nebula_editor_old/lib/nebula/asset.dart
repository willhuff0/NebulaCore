import 'package:flutter/material.dart';
import 'package:meta/meta.dart';
import 'package:nebula_editor/nebula.dart';
import 'package:uuid/uuid.dart';

export 'assets/scene.dart';
export 'assets/texture.dart';
export 'assets/material.dart';

abstract class NbAsset {
  final String name;
  final String guid;

  String get groupKey;

  NbAsset.create(this.name) : guid = Uuid().v4();

  NbAsset.deserialize(this.guid, Map<String, dynamic> json) : name = json['name'] as String;

  static NbAsset from(String group, String guid, Map<String, dynamic> json) {
    return deserializers[group]!(guid, json);
  }

  static const deserializers = <String, NbAsset Function(String guid, Map<String, dynamic> json)>{
    'scenes': NbSceneAsset.deserialize,
    'textures': NbTextureAsset.deserialize,
    'meterials': NbMaterialAsset.deserialize,
  };

  @mustCallSuper
  Map<String, dynamic> serialize() => {
        'name': name,
      };

  Future<void> sendChanges() async {
    await Nebula.peer.sendRequest('SyncAsset', {
      'group': groupKey,
      'guid': guid,
      'asset': serialize(),
    });
  }
}

abstract class NbFileAsset extends NbAsset {
  final String path;

  NbFileAsset.create(super.name, this.path) : super.create();

  NbFileAsset.deserialize(super.guid, super.json)
      : path = json['path'],
        super.deserialize();

  @override
  @mustCallSuper
  Map<String, dynamic> serialize() => {
        ...super.serialize(),
        'path': path,
      };
}
