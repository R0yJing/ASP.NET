#!/usr/bin/python
from PIL import Image, ImageEnhance
from pathlib import Path
import os
import numpy as np
import matplotlib as plt
import gc
currentDir = os.getcwd()
TempFolder = os.path.join(currentDir, "TempImageFolder")
allMaligns = r"malignant"
allBenigns = r"benign"
allUnknowns = r"unknown"
class ImageWrapper:
    def __init__(self, img, name):
        self.filename = name
        self.img = img
def equaliseOne(img :Image):
    matrix = np.array(img)
    flattened = matrix.flatten()
    histogram_array = np.bincount(flattened, minlength=256)
    #normalize
    num_pixels = np.sum(histogram_array)
    histogram_array = histogram_array/num_pixels
    #cumulative histogram
    chistogram_array = np.cumsum(histogram_array)
    """
    STEP 2: Pixel mapping lookup table
    """
    transform_map = np.floor(255 * chistogram_array).astype(np.uint8)
    """
    STEP 3: Transformation
    """
    # flatten image array into 1D list

    # transform pixel values to equalize
    eq_img_list = [transform_map[p] for p in flattened]
    # reshape and write back into img_array
    eq_img_array = np.reshape(np.asarray(eq_img_list), matrix.shape)

    return Image.fromarray(eq_img_array)
def equalise(imageSet):
    processed = []
    for wrapper in imageSet:
        image = wrapper.img

        imageRawData =  equaliseOne(image)
        #im = Image.fromarray(imageRawData)
        processed.append(ImageWrapper(imageRawData, wrapper.filename))
    
    return processed


def contrastEnhance(imageSet):

    processed =[]
    for wrapper in imageSet:
        img = wrapper.img
        #image brightness enhancer
        enhancer = ImageEnhance.Contrast(img)
        factor = 1.2 #increase contrast
        im_output = enhancer.enhance(factor)
       
        #with file extension
        '''try:
            os.remove(os.path.join(TempFolder, file))
        except OSError:
            print("unable to remove " + file)'''
        #im_output.save(os.path.join(TempFolder, "Contrasted_" + file))
        processed.append(ImageWrapper(im_output, wrapper.filename))
    return processed


def sharpen(imageSet):
    processed = []
    
    for wrapper in imageSet:
        img = wrapper.img
        enhancer = ImageEnhance.Sharpness(img)
        enhanced_im = enhancer.enhance(10.0)
        img.close()
        #enhanced_im.save(os.path.join(TempFolder, "Sharpened_"+img ))
        processed.append(ImageWrapper(enhanced_im, wrapper.filename))

    return processed

#powerset 
def generateAllCombination(idx, imageSet, funcs, funcsNameCalled):
    if idx == len(funcs):
        dirName = os.path.join(TempFolder, funcsNameCalled)
        if not os.path.exists(dirName):
            os.mkdir(dirName)
        for wrapper in imageSet:
            filename = os.path.basename(wrapper.filename)
            wrapper.img.save(os.path.join(dirName,funcsNameCalled+ filename))
     
    else:
        #generateAll(idx + 1, imageSet, funcs, funcsNameCalled)
        processedImg, funcCalled = funcs[idx](imageSet)
    
        #generateAll(idx + 1, processedImg, funcs, funcCalled + funcsNameCalled)
def enhanceAll(imageSet, funcs, foldername):
    foldername = "augmented_" + foldername
    for func in funcs:
        imageSet = func(imageSet)
        
        gc.collect()
    folderpath= os.path.join(TempFolder, foldername)
    if not os.path.exists(os.path.join(TempFolder, foldername)):
        os.mkdir(os.path.join(TempFolder, foldername))
    for wrapper in imageSet:
        wrapper.img.save(os.path.join(folderpath, wrapper.filename))
            
def getImages(folder):
    imageSet = []
    numFiles = 0
    for filename in os.listdir(folder):
        
        if filename.endswith(".jpg") or filename.endswith(".png"):
            filepath = os.path.join(folder, filename)
            img = Image.open(filepath)
            wrapper = ImageWrapper(img, filename)
            imageSet.append(wrapper)
    return imageSet

def testGetImage():
    ims = getImages(TempFolder)
    assert ims.__len__ !=0
    return ims
def compare(images: ImageWrapper, func):
    new =  func(images)
    
    for img, newImg in zip(images, new):
        if list(img.img.getdata()) == list(newImg.img.getdata()):
            return False
    return True

def testSharpen(images):
    assert(compare(images, sharpen))

def testContrast(images : ImageWrapper): 
    assert(compare(images, contrastEnhance))

def testEqualise(images):
    assert(compare(images, equalise))

if __name__=='__main__':
    funcs = [equalise, contrastEnhance, sharpen]
    im = getImages(allMaligns)
    enhanceAll(im, funcs, allMaligns)
    im = getImages(allBenigns)
    enhanceAll(im, funcs, allBenigns)
    im = getImages(allUnknowns)
    enhanceAll(im, funcs, allUnknowns)

    #testSharpen(imgs)
    #enhanceAll(getImages(allUnknowns), funcArr, "unknown_augmented")
    #enhanceAll(getImages(allMaligns), funcArr, "malign_augmented")

    #generateAll(0, getImages(), funcArr, "")
    
    # imgs = getImages()
    # testSharpen(imgs)
    # contrastEnhance(imgs)
    # Equaliser.equalise(imgs)
    
