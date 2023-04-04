import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart' as mt;
import 'package:nebula/nebula.dart';

import 'package:http/http.dart' as http;

class EditorContext {
  final void Function() _onInitContext;

  Nebula? _context;
  Nebula? get context => _context;
  Nebula get nebula => context!;

  late final StreamController<EditorLogEntry> _onLogController;
  late final Stream<EditorLogEntry> onLog;

  late final StreamController<NebulaProject?> _onProjectLoadedController;
  late final Stream<NebulaProject?> onProjectLoaded;

  late final StreamController<Scene?> _onSceneLoadedController;
  late final Stream<Scene?> onSceneLoaded;

  EditorContext(this._onInitContext) {
    _onLogController = StreamController.broadcast();
    _onProjectLoadedController = StreamController.broadcast();
    _onSceneLoadedController = StreamController.broadcast();
    onLog = _onLogController.stream;
    onProjectLoaded = _onProjectLoadedController.stream;
    onSceneLoaded = _onSceneLoadedController.stream;
  }

  void initContext(Nebula nebulaContext) {
    _context = nebulaContext;
    _onInitContext();
    log('Initialized Nebula ${nebula.getVersion()}\nAPI: ${nebula.getGlVersion()}\nDevice: ${nebula.getRenderer()}');
  }

  Future<void> openProject() async {
    final project = await nebula.openNebulaProject();
    _onProjectLoadedController.add(project);
  }

  Future<void> loadProject(NebulaFileSystem fileSystem) async {
    final project = await NebulaProject.deserializeWithImports(json.decode(await fileSystem.readProjectFile()) as Json, fileSystem);
    nebula.activeProject = project;
    _onProjectLoadedController.add(project);
  }

  Future<void> createProject(String name, String bundleId) async {
    final project = await nebula.createNebulaProject(name, bundleId);
    final builtIn = (await http.get(Uri.parse('nebula/built_in.neb'))).body;
    await project!.fileSystem.writeFileAsString('deps/built_in.neb', builtIn);
    await project.addImports(['deps/built_in.neb']);
    await project.saveProjectFile();
    _onProjectLoadedController.add(project);
  }

  void log(String message, {LogLevel level = LogLevel.info}) {
    _onLogController.add(EditorLogEntry(message, level));
  }
}

class EditorLogEntry {
  final String message;
  final LogLevel level;
  final DateTime timestamp;

  EditorLogEntry(this.message, this.level) : timestamp = DateTime.now();
}

enum LogLevel {
  info('Info', mt.Icons.info_rounded, null),
  alert('Alert', mt.Icons.warning_rounded, mt.Color(0xFFFF6F00)),
  error('Error', mt.Icons.error_rounded, mt.Colors.red),
  fatal('Fatal Error', mt.Icons.cancel_rounded, mt.Color(0xFFB71C1C));

  final String name;
  final mt.IconData icon;
  final mt.Color? color;

  const LogLevel(this.name, this.icon, this.color);
}
