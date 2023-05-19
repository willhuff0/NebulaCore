import 'package:nebula_editor/nebula.dart';

class NbMaterialAsset extends NbAsset {
  String _shader;
  String get shader => _shader;
  final Map<String, String> _textures;
  Map<String, String> get textures => _textures;
  final Map<String, dynamic> _params;

  NbMaterialAsset.create(super.name, this._shader, [this._textures = const {}, this._params = const {}]) : super.create();

  NbMaterialAsset.deserialize(super.guid, super.json)
      : _shader = json['shader'] as String,
        _textures = json['textures'] as Map<String, String>,
        _params = json['params'] as Map<String, dynamic>,
        super.deserialize();

  @override
  String get groupKey => 'materials';

  @override
  Map<String, dynamic> serialize() => {
        ...super.serialize(),
        'shader': _shader,
        if (_textures.isNotEmpty) 'textures': _textures,
        if (_params.isNotEmpty) 'params': _params,
      };
}
