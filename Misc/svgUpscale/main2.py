import os
import cairosvg
from PIL import Image

def convert_svg_to_png(svg_path, png_path):
    cairosvg.svg2png(url=svg_path, write_to=png_path)

def upscale_png(png_path, scale_factor):
    with Image.open(png_path) as img:
        new_size = (int(img.width * scale_factor), int(img.height * scale_factor))
        upscaled_img = img.resize(new_size, Image.ANTIALIAS)
        upscaled_img.save(png_path)

def process_svgs(folder_path, scale_factor=2):
    upscaled_folder = os.path.join(folder_path, 'upscaledPNGs')
    os.makedirs(upscaled_folder, exist_ok=True)

    for filename in os.listdir(folder_path):
        if filename.endswith('.svg'):
            svg_path = os.path.join(folder_path, filename)
            png_filename = filename.replace('.svg', '.png')
            png_path = os.path.join(upscaled_folder, png_filename)

            try:
                # Convert SVG to PNG
                convert_svg_to_png(svg_path, png_path)
                print(f"Converted {filename} to PNG")

                # Upscale the PNG
                upscale_png(png_path, scale_factor)
                print(f"Upscaled {png_filename}")
            except Exception as e:
                print(f"Error processing {filename}: {e}")

# Example usage
folder_path = 'path_to_your_svg_folder'
process_svgs(folder_path)
