import 'dart:async';
import 'dart:convert';
import 'dart:io';

import 'package:json_rpc_2/json_rpc_2.dart';
import 'package:stream_channel/stream_channel.dart';

export 'nebula/project.dart';
export 'nebula/asset.dart';
export 'nebula/node.dart';

class Nebula {
  static late final Socket _socket;
  static late final Peer peer;
  static bool _connected = false;
  static bool get connected => _connected;

  static Future connectToDebugger(String key, {int port = 3590}) async {
    _socket = await Socket.connect('127.0.0.1', port);
    peer = Peer(StreamChannel(_socket, _socket).transform(StreamChannelTransformer.fromCodec(const Utf8Codec())));
    peer.registerMethod('OnError', (Parameters params) {});
    peer.listen().then((value) async {
      _connected = false;
      await _socket.close();
    });
    _connected = true;
  }

  Future<void> disconnectFromDebugger() async => await peer.close();
}
