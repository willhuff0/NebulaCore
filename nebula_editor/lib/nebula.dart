import 'dart:async';
import 'dart:collection';
import 'dart:convert';
import 'dart:io';

import 'package:json_rpc_2/json_rpc_2.dart';
import 'package:stream_channel/stream_channel.dart';

typedef NodePropertiesMap = Map<String, Map<String, dynamic>>;

class Nebula {
  final Socket _socket;
  late final Client _client;

  Nebula._(this._socket) {
    _client = Client(StreamChannel(_socket, _socket).transform(StreamChannelTransformer.fromCodec(const Utf8Codec())));
    _client.listen().then((value) => _socket.close());
  }

  static Future<Nebula> connectToDebugger(String key, {int port = 3590}) async {
    final socket = await Socket.connect('127.0.0.1', port);
    return Nebula._(socket);
  }

  Future<void> disconnectFromDebugger() => _client.close();

  Future<NodePropertiesMap> getNodeProperties(String guid) async {
    return await _client.sendRequest('GetNodeProperties', {
      'guid': guid,
    });
  }

  void setNodeProperty(String guid, String behavior, String path, dynamic value) {
    _client.sendNotification('SetNodeProperty', {
      'guid': guid,
      'behavior': behavior,
      'path': path,
      'value': value,
    });
  }
}

class NebulaNode {
  final Nebula _nebula;

  final String guid;
  NodePropertiesMap _properties;

  NebulaNode(this._nebula, this.guid) : _properties = {};

  UnmodifiableMapView get properties => UnmodifiableMapView(_properties);

  Future<void> syncProperties() async => _properties = await _nebula.getNodeProperties(guid);

  void setProperty(String behavior, String path, dynamic value) => _nebula.setNodeProperty(guid, behavior, path, value);
}
