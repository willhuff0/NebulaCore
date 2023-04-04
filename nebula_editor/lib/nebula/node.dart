import 'package:nebula_editor/nebula.dart';

class NbNode {
  final String guid;
  Map<String, Map<String, dynamic>> _properties;

  NbNode._(this.guid, this._properties);

  Map<String, Map<String, dynamic>> get properties => Map.unmodifiable(_properties);

  static Future<NbNode> getNode(String guid) async {
    final properties = await Nebula.peer.sendRequest('GetNodeProperties', {'guid': guid});
    return NbNode._(guid, properties);
  }

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
