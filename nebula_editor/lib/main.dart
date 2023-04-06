import 'package:flutter/material.dart';
import 'package:nebula_editor/nebula.dart';

import 'editor/editor_page.dart';

void main() {
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

class _HomeState extends State<Home> {
  var ready = false;

  @override
  void initState() {
    Nebula.connectToDebugger('nebula-dev-key').then((value) => setState(() => ready = true));
    super.initState();
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
}
