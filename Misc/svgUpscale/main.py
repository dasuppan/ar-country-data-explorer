import os
import shutil
from xml.etree import ElementTree as ET

def apply_transform(elem, scale_factor):
    """Apply scaling transform to SVG elements."""
    transform = elem.attrib.get('transform', '')
    if transform:
        transform = f'{transform} scale({scale_factor},{scale_factor})'
    else:
        transform = f'scale({scale_factor},{scale_factor})'
    elem.attrib['transform'] = transform

    for child in elem:
        apply_transform(child, scale_factor)

def upscale_svg(svg_content, scale_factor):
    # Parse the SVG content
    tree = ET.ElementTree(ET.fromstring(svg_content))
    ET.register_namespace("", "http://www.w3.org/2000/svg")
    root = tree.getroot()

    # Update the width and height attributes
    width = root.attrib.get('width')
    height = root.attrib.get('height')

    if width and height:
        try:
            new_width = str(float(width) * scale_factor)
            new_height = str(float(height) * scale_factor)
            root.attrib['width'] = new_width
            root.attrib['height'] = new_height
        except ValueError:
            pass

    # Update the viewBox attribute
    viewBox = root.attrib.get('viewBox')
    if viewBox:
        try:
            min_x, min_y, w, h = map(float, viewBox.split())
            new_viewBox = f"{min_x} {min_y} {w * scale_factor} {h * scale_factor}"
            root.attrib['viewBox'] = new_viewBox
        except ValueError:
            pass

    # Apply scaling transform to all elements
    apply_transform(root, scale_factor)

    return ET.tostring(root, encoding='unicode')


def upscale_svgs_in_folder(folder_path, scale_factor=2):
    upscaled_folder = os.path.join(folder_path, 'upscaled')
    os.makedirs(upscaled_folder, exist_ok=True)

    for filename in os.listdir(folder_path):
        if filename.endswith('.svg'):
            file_path = os.path.join(folder_path, filename)
            try:
                with open(file_path, 'r', encoding='utf-8') as file:
                    svg_content = file.read()

                upscaled_svg = upscale_svg(svg_content, scale_factor)

                upscaled_file_path = os.path.join(upscaled_folder, filename)
                with open(upscaled_file_path, 'w', encoding='utf-8') as file:
                    file.write(upscaled_svg)

                print(f"Upscaled and saved: {filename}")

            except Exception as e:
                print(f"Error processing {filename}: {e}")


folder_path = 'C:/Users/david/Desktop/ARProject/ARProject/Assets/Resources/Flags/4x3'
upscale_svgs_in_folder(folder_path)
