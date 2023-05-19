import 'package:nebula_editor/nebula.dart';

class NbNode {
  final String guid;
  final List<NbNode> _children;
  Map<String, Map<String, dynamic>> _properties;

  NbNode._(this.guid, this._children, this._properties);

  List<NbNode> get children => List.unmodifiable(_children);
  Map<String, Map<String, dynamic>> get properties => Map.unmodifiable(_properties);

  NbNode.deserialize(Map<String, dynamic> json)
      : guid = json['guid'] as String,
        _children = (json['children'] as List<Map<String, dynamic>>?)?.map((e) => NbNode.deserialize(e)).toList() ?? [],
        _properties = (json['properties'] as Map<String, Map<String, dynamic>>?) ?? {};

  Future<void> syncProperties() async {
    _properties = await Nebula.peer.sendRequest('GetNodeProperties', {'guid': guid});
  }

  void setProperty(String behavior, String path, dynamic value) {
    (_properties[behavior] ??= <String, dynamic>{})[path] = value;
    Nebula.peer.sendNotification('SetNodeProperty', {
      'guid': guid,
      'behavior': behavior,
      'path': path,
      'value': value,
    });
  }
}
