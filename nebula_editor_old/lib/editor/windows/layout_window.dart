import 'dart:async';

import 'package:flutter/material.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:nebula_editor/nebula.dart';

class EditorWindowLayout extends StatefulWidget {
  const EditorWindowLayout({super.key});

  @override
  State<EditorWindowLayout> createState() => _EditorWindowLayoutState();
}

class _EditorWindowLayoutState extends State<EditorWindowLayout> {
  late final StreamSubscription _onSceneLoadedSubscription;

  NbScene? scene;

  @override
  void initState() {
    _onSceneLoadedSubscription = EditorContext.onSceneLoaded.listen((event) => setState(() => scene = event));
    super.initState();
  }

  @override
  void dispose() {
    _onSceneLoadedSubscription.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (scene == null) {
      return Center(child: Text('No scene loaded'));
    }
    return scene.
  }
}
