const fs = require('fs');
const path = require('path');
const sharp = require('sharp');

async function convertAndUpscaleSVG(svgPath, pngPath, targetHeight) {
  try {
    const svgBuffer = fs.readFileSync(svgPath);
    
    // Convert SVG to PNG and upscale
    await sharp(svgBuffer)
      .resize({ height: Math.ceil(targetHeight) })
      .toFile(pngPath);
  } catch (error) {
    console.error(`Error processing ${svgPath}:`, error);
  }
}

async function processSvgs(folderPath, targetHeight = 300) {
  const upscaledFolder = path.join(folderPath, 'upscaled');
  if (!fs.existsSync(upscaledFolder)) {
    fs.mkdirSync(upscaledFolder);
  }

  const files = fs.readdirSync(folderPath);

  for (const filename of files) {
    if (filename.endsWith('.svg')) {
      const svgPath = path.join(folderPath, filename);
      const pngFilename = filename.replace('.svg', '.png');
      const pngPath = path.join(upscaledFolder, pngFilename);

      await convertAndUpscaleSVG(svgPath, pngPath, targetHeight);
      console.log(`Converted and upscaled: ${filename}`);
    }
  }
}

// Example usage
const folderPath = 'C:/Users/david/Desktop/ARProject/ARProject/Assets/Resources/Flags/4x3';
processSvgs(folderPath);
