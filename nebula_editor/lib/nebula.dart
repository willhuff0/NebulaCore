import 'dart:async';

import 'package:json_rpc_2/json_rpc_2.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:web_socket_channel/web_socket_channel.dart';

export 'nebula/project.dart';
export 'nebula/asset.dart';
export 'nebula/node.dart';
export 'nebula/scene.dart';

class Nebula {
  static late final WebSocketChannel _socket;
  static late final Peer peer;
  static bool _connected = false;
  static bool get connected => _connected;

  static Future<void> connectToDebugger(String key, {int port = 3590}) async {
    _socket = WebSocketChannel.connect(Uri.parse('ws://127.0.0.1:$port'));
    peer = Peer(_socket.cast<String>());

    peer.registerMethod('OnError', (Parameters params) {});

    peer.listen().then((value) async {
      _connected = false;
      await _socket.sink.close();
    });

    _connected = true;

    getEngineInfo().then((info) => EditorContext.log('Initialized Nebula ${info.nebulaVersion}\nAPI: ${info.glVersion}\nDevice: ${info.glRenderer}'));
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
