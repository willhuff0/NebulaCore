import 'package:collection/collection.dart';
import 'package:flutter/material.dart';

class EditorZone extends StatefulWidget {
  final List<EditorWindow> tabs;

  const EditorZone({super.key, required this.tabs});

  @override
  State<EditorZone> createState() => _EditorZoneState();
}

class _EditorZoneState extends State<EditorZone> {
  final tabs = <EditorWindow>[];
  var selectedIndex = 0;

  @override
  void initState() {
    tabs.addAll(widget.tabs);
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8.0)),
      margin: EdgeInsets.all(4.0),
      child: Padding(
        padding: const EdgeInsets.only(left: 4.0, bottom: 4.0, right: 4.0, top: 4.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 6.0),
              child: Wrap(
                crossAxisAlignment: WrapCrossAlignment.end,
                spacing: 2.0,
                runSpacing: 2.0,
                children: tabs.mapIndexed(
                  (i, e) {
                    return SizedBox(
                      height: 20.0,
                      child: FilledButton.tonal(
                        style: ButtonStyle(
                          shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(4.0)))),
                          padding: MaterialStatePropertyAll(EdgeInsets.symmetric(vertical: 2.0)),
                        ),
                        clipBehavior: Clip.antiAlias,
                        onPressed: () {
                          if (i != selectedIndex) {
                            setState(() => selectedIndex = i);
                          }
                        },
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            i == selectedIndex
                                ? Container(
                                    width: 3.0,
                                    color: Theme.of(context).colorScheme.primary,
                                    margin: EdgeInsets.only(right: 6.0),
                                  )
                                : SizedBox(width: 9.0),
                            Text(e.config.name),
                            SizedBox(width: 4.0),
                            IconButton(
                              onPressed: () {
                                print('Close ${e.config.name}');
                              },
                              icon: Icon(Icons.close),
                              splashRadius: 2.0,
                              iconSize: 12.0,
                              visualDensity: VisualDensity.compact,
                              padding: EdgeInsets.zero,
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ).toList(),
              ),
            ),
            Expanded(
              child: IndexedStack(
                index: selectedIndex,
                children: tabs,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class EditorWindowConfig {
  final String name;
  final Widget child;

  EditorWindowConfig(this.name, this.child);
}

class EditorWindow extends StatelessWidget {
  final EditorWindowConfig config;

  const EditorWindow({super.key, required this.config});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.background,
        borderRadius: BorderRadius.circular(4.0),
      ),
      clipBehavior: Clip.antiAlias,
      child: config.child,
    );
  }
}
