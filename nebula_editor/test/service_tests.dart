import 'package:flutter_test/flutter_test.dart';
import 'package:nebula_editor/services.dart';

void main() {
  group('texture', () {
    test('compression_tests', () async {
      // 2048x2048 srgb albedo

      print('Started compression_tests');

      await TextureService.losslessCompressTexture('/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/albedo.png', '/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/test.jxl');
      print('Lossless compressed texture as jxl');

      await TextureService.losslessDecompressTexture('/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/test.jxl', '/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/test.png');
      print('Lossless decompressed texture as png');

      var mebibyteToBits = 8388608;
      final quality = TextureService.estimateAstcQuality(
        2048,
        2048,
        1 * mebibyteToBits,
      );

      print('Estimated ASTC Quality: $quality');

      await TextureService.lossyBundleTexture('/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/test.png', '/Users/will/RiderProjects/NebulaCore/TestProject/assets/textures/test.astc', quality, true);
      print('Lossy bundled texture as astc');
    });
  });
}
