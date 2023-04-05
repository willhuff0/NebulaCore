import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'dart:math';

import 'package:json_rpc_2/json_rpc_2.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:stream_channel/stream_channel.dart';

export 'nebula/project.dart';
export 'nebula/asset.dart';
export 'nebula/node.dart';
export 'nebula/scene.dart';

class Nebula {
  static late final Socket _socket;
  static late final Peer peer;
  static bool _connected = false;
  static bool get connected => _connected;

  static Future connectToDebugger(String key, {int port = 3590}) async {
    _socket = await Socket.connect('127.0.0.1', port);
    peer = Peer(StreamChannel(_socket, _socket).transform(StreamChannelTransformer.fromCodec(const Utf8Codec())));

    peer.registerMethod('OnError', (Parameters params) {
      
    });

    peer.listen().then((value) async {
      _connected = false;
      await _socket.close();
    });

    _connected = true;

    getEngineInfo().then((value) => log('Initialized Nebula ${value.nebulaVersion}\nAPI: ${nebula.getGlVersion()}\nDevice: ${nebula.getRenderer()}');)
  }

  static Future<void> disconnectFromDebugger() async => await peer.close();

  static Future<NbInfo> getEngineInfo() async {
    return NbInfo.deserialize(await peer.sendRequest('GetEngineInfo'));
  }
}

class NbInfo {
  final String nebulaVersion;
  final String glVersion;
  final String glRenderer;

  NbInfo._(this.nebulaVersion, this.glVersion, this.glRenderer);

  NbInfo.deserialize(Map<String, dynamic> json) 
  : nebulaVersion = json['nebulaVersion'] as String, 
  glVersion = json['glVersion'] as String, 
  glRenderer = json['glRenderer'] as String;
}
