import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula_widgets.dart';

import '../editor_page.dart';

class EditorWindowViewport extends StatefulWidget {
  const EditorWindowViewport({super.key});

  @override
  State<EditorWindowViewport> createState() => _EditorWindowViewportState();
}

class _EditorWindowViewportState extends State<EditorWindowViewport> {
  late final GlobalKey viewKey;

  @override
  void initState() {
    viewKey = GlobalKey();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return NebulaViewWeb(key: viewKey, onNebulaEngineInit: (nebula) => Editor.of(context).initContext(nebula));
  }
}
