import 'dart:async';

import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula.dart';

class EditorContext {
  static NbProject? _activeProject;
  static NbProject? get activeProject => _activeProject;
  static set activeProject(NbProject? value) {
    _activeProject = value;
    _onProjectLoadedController.add(_activeProject);
    if (value != null) log('Active project: \'${value.name}\'', level: LogLevel.engine);
  }

  static NbScene? _activeScene;
  static NbScene? get activeScene => _activeScene;
  static set activeScene(NbScene? value) {
    _activeScene = value;
    _onSceneLoadedController.add(_activeScene);
    if (value != null) log('Active scene: \'${value.name}\'', level: LogLevel.engine);
  }

  static final _onLogController = StreamController<LogEntry>.broadcast();
  static final onLog = _onLogController.stream;

  static final _onProjectLoadedController = StreamController<NbProject?>.broadcast();
  static final onProjectLoaded = _onProjectLoadedController.stream;

  static final _onSceneLoadedController = StreamController<NbScene?>.broadcast();
  static final onSceneLoaded = _onSceneLoadedController.stream;

  static void log(String message, {LogLevel level = LogLevel.info}) {
    print('Log ($level): $message');
    _onLogController.add(LogEntry(message, level));
  }
}

class LogEntry {
  final String message;
  final LogLevel level;
  final DateTime timestamp;

  LogEntry(this.message, this.level) : timestamp = DateTime.now();
}

enum LogLevel {
  info('Info', Icons.info_rounded, null),
  engine('Engine', Icons.settings_rounded, Color(0xFF7E57C2)),
  alert('Alert', Icons.warning_rounded, Color(0xFFFF6F00)),
  error('Error', Icons.error_rounded, Colors.red),
  fatal('Fatal Error', Icons.cancel_rounded, Color(0xFFB71C1C));

  final String name;
  final IconData icon;
  final Color? color;

  const LogLevel(this.name, this.icon, this.color);
}
