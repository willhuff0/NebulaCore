import 'dart:async';

import 'package:flutter/material.dart';

import '../editor_context.dart';
import '../editor_page.dart';

class EditorWindowDebugger extends StatefulWidget {
  const EditorWindowDebugger({super.key});

  @override
  State<EditorWindowDebugger> createState() => _EditorWindowDebuggerState();
}

class _EditorWindowDebuggerState extends State<EditorWindowDebugger> {
  StreamSubscription? _onLogSubscription;
  late final List<LogEntry> entries;
  late List<LogEntry> filteredEntries;

  late final Set<LogLevel> levelFilters;

  @override
  void initState() {
    entries = [];
    filteredEntries = [];
    levelFilters = LogLevel.values.toSet();
    WidgetsBinding.instance.addPostFrameCallback((timeStamp) => _onLogSubscription = EditorContext.onLog.listen((entry) => setState(() {
          entries.insert(0, entry);
          if (levelFilters.contains(entry.level)) filteredEntries.insert(0, entry);
        })));
    super.initState();
  }

  void filterEntries() {
    filteredEntries = entries.where((entry) => levelFilters.contains(entry.level)).toList();
  }

  @override
  void dispose() {
    _onLogSubscription?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 28.0,
          color: Theme.of(context).colorScheme.surfaceTint.withOpacity(0.1),
          padding: EdgeInsets.symmetric(vertical: 4.0),
          child: Column(
            children: [
              MenuAnchor(
                style: MenuStyle(alignment: Alignment.topRight, visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                alignmentOffset: Offset(4.0, 0.0),
                menuChildren: [
                  SizedBox(
                    width: 100.0,
                    child: TextButton.icon(
                      style: ButtonStyle(
                        shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(4.0))),
                        alignment: Alignment.centerLeft,
                      ),
                      icon: Icon(levelFilters.contains(LogLevel.info) ? Icons.visibility_rounded : Icons.visibility_off_rounded, size: 20.0),
                      label: Text('Info'),
                      onPressed: () {
                        setState(() {
                          if (!levelFilters.remove(LogLevel.info)) {
                            levelFilters.add(LogLevel.info);
                          }
                          filterEntries();
                        });
                      },
                    ),
                  ),
                  SizedBox(
                    width: 100.0,
                    child: TextButton.icon(
                      style: ButtonStyle(
                        shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(4.0))),
                        alignment: Alignment.centerLeft,
                        foregroundColor: MaterialStatePropertyAll(LogLevel.alert.color),
                      ),
                      icon: Icon(levelFilters.contains(LogLevel.alert) ? Icons.visibility_rounded : Icons.visibility_off_rounded, size: 20.0),
                      label: Text('Alert'),
                      onPressed: () {
                        setState(() {
                          if (!levelFilters.remove(LogLevel.alert)) {
                            levelFilters.add(LogLevel.alert);
                          }
                          filterEntries();
                        });
                      },
                    ),
                  ),
                  SizedBox(
                    width: 100.0,
                    child: TextButton.icon(
                      style: ButtonStyle(
                        shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(4.0))),
                        alignment: Alignment.centerLeft,
                        foregroundColor: MaterialStatePropertyAll(LogLevel.error.color),
                      ),
                      icon: Icon(levelFilters.contains(LogLevel.error) ? Icons.visibility_rounded : Icons.visibility_off_rounded, size: 20.0),
                      label: Text('Error'),
                      onPressed: () {
                        setState(() {
                          if (!levelFilters.remove(LogLevel.error)) {
                            levelFilters.add(LogLevel.error);
                          }
                          filterEntries();
                        });
                      },
                    ),
                  ),
                  SizedBox(
                    width: 100.0,
                    child: TextButton.icon(
                      style: ButtonStyle(
                        shape: MaterialStatePropertyAll(RoundedRectangleBorder(borderRadius: BorderRadius.circular(4.0))),
                        alignment: Alignment.centerLeft,
                        foregroundColor: MaterialStatePropertyAll(LogLevel.fatal.color),
                      ),
                      icon: Icon(levelFilters.contains(LogLevel.fatal) ? Icons.visibility_rounded : Icons.visibility_off_rounded, size: 20.0),
                      label: Text('Fatal'),
                      onPressed: () {
                        setState(() {
                          if (!levelFilters.remove(LogLevel.fatal)) {
                            levelFilters.add(LogLevel.fatal);
                          }
                          filterEntries();
                        });
                      },
                    ),
                  ),
                ],
                builder: (context, controller, child) => IconButton(
                  onPressed: () => controller.open(),
                  padding: EdgeInsets.zero,
                  iconSize: 20.0,
                  tooltip: 'Filter',
                  icon: Icon(Icons.visibility_rounded),
                ),
              ),
              Expanded(child: Container()),
              MenuAnchor(
                style: MenuStyle(alignment: Alignment.topRight, visualDensity: VisualDensity(horizontal: VisualDensity.minimumDensity, vertical: VisualDensity.minimumDensity)),
                menuChildren: [
                  MenuItemButton(
                    child: Text('Clear', style: TextStyle(color: Theme.of(context).colorScheme.error)),
                    onPressed: () {
                      setState(() {
                        entries.clear();
                        filterEntries();
                      });
                    },
                  ),
                ],
                builder: (context, controller, child) => IconButton(
                  icon: Icon(Icons.delete_forever_rounded),
                  iconSize: 20.0,
                  tooltip: 'Clear',
                  padding: EdgeInsets.zero,
                  onPressed: () => controller.open(),
                ),
              ),
            ],
          ),
        ),
        Expanded(
          child: ListView.builder(
            reverse: true,
            itemCount: filteredEntries.length,
            itemBuilder: (context, index) {
              final entry = filteredEntries[index];
              if (!levelFilters.contains(entry.level)) return null;
              return Container(
                color: entry.level.color?.withOpacity(0.05) ?? (index.isOdd ? Theme.of(context).colorScheme.surfaceTint.withOpacity(0.03) : null),
                child: ListTile(
                  leading: Icon(entry.level.icon, color: entry.level.color),
                  titleAlignment: ListTileTitleAlignment.top,
                  title: Text('[${entry.timestamp}]', style: Theme.of(context).textTheme.bodySmall?.copyWith(color: Theme.of(context).textTheme.bodySmall?.color?.withOpacity(0.5))),
                  subtitle: Padding(
                    padding: const EdgeInsets.only(left: 4.0, top: 2.0),
                    child: Text(entry.message),
                  ),
                  minLeadingWidth: 0.0,
                  dense: true,
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}
