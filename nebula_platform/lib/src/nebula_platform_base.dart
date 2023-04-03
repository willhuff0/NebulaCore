import 'dart:convert';
import 'dart:io';

import 'package:json_rpc_2/json_rpc_2.dart';

typedef Json = Map<String, dynamic>;

class Nebula {
  final Socket _socket;
  late final Client _client;

  Nebula._(this._socket) {
    _socket.map((event) => json.decode(utf8.decode(event)) as Json).listen(_onMessage);

    var client = Client(_socket.cast<String>());
  }

  static Future<Nebula> connectToDebugger(String key, {int port = 3590}) async {
    final socket = await Socket.connect('127.0.0.1', port);
    return Nebula._(socket);
  }
}
