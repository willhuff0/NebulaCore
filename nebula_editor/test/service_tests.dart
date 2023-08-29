import 'package:flutter_test/flutter_test.dart';
import 'package:nebula_editor/services.dart';

void main() {
  group('astc', () {
    test('astc_getCompressionQuality', () {
      // 2048x2048 linear albedo webp

      var mebibyteToBits = 8388608;

      ModelService.getCompressionQuality(
        2048,
        2048,
      );
    });
  });
}
