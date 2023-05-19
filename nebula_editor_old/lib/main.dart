import 'package:flutter/material.dart';
import 'package:nebula_editor/editor/editor_context.dart';
import 'package:nebula_editor/nebula.dart';
import 'package:window_manager/window_manager.dart';

import 'editor/editor_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await windowManager.ensureInitialized();
  WindowOptions windowOptions = WindowOptions(
    backgroundColor: Colors.transparent,
    titleBarStyle: TitleBarStyle.hidden,
  );
  windowManager.waitUntilReadyToShow(windowOptions, () async {
    await windowManager.show();
  });
  runApp(App());
}

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Home(),
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        brightness: Brightness.dark,
        useMaterial3: true,
        menuTheme: MenuThemeData(
          style: MenuStyle(
            visualDensity: VisualDensity(vertical: VisualDensity.minimumDensity, horizontal: VisualDensity.minimumDensity),
          ),
        ),
      ),
    );
  }
}

class Home extends StatefulWidget {
  const Home({super.key});

  @override
  State<Home> createState() => _HomeState();
}

class _HomeState extends State<Home> with WindowListener {
  var ready = false;

  @override
  void initState() {
    windowManager.addListener(this);
    windowManager.setPreventClose(true);
    Nebula.connectToDebugger('nebula-dev-key').then((value) => setState(() => ready = true));
    super.initState();
  }

  @override
  void dispose() {
    windowManager.removeListener(this);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ready
        ? Editor()
        : Scaffold(
            body: Center(
              child: Card(
                child: SizedBox(
                  width: 400.0,
                  height: 300.0,
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text('Connecting to engine'),
                      SizedBox(height: 36.0),
                      CircularProgressIndicator(),
                      //SizedBox(height: 24.0),
                      //SizedBox(width: 200.0, child: LinearProgressIndicator()),
                    ],
                  ),
                ),
              ),
            ),
          );
  }

  @override
  void onWindowClose() async {
    final preventClose = await windowManager.isPreventClose();
    if (EditorContext.activeProject == null || !await NbProject.hasChanges()) {
      await windowManager.destroy();
      return;
    }
    if (!mounted) return;
    if (preventClose) {
      showDialog(
        context: context,
        builder: (_) {
          return AlertDialog(
            title: Text('Are you sure you want to close this window without saving changes?'),
            actions: [
              TextButton(
                child: Text('No'),
                onPressed: () {
                  Navigator.of(context).pop();
                },
              ),
              TextButton(
                child: Text('Yes'),
                onPressed: () async {
                  Navigator.of(context).pop();
                  await windowManager.destroy();
                },
              ),
            ],
          );
        },
      );
    }
  }
}
