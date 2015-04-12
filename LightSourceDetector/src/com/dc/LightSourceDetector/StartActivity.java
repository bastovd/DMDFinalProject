package com.dc.LightSourceDetector;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.AssetManager;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.ImageFormat;
import android.graphics.Point;
import android.graphics.PointF;
import android.graphics.Rect;
import android.graphics.YuvImage;
import android.hardware.Camera;
import android.media.FaceDetector;
import android.os.Bundle;
import android.os.Environment;
import android.os.Handler;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowManager;
import android.widget.FrameLayout;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Created by Anton on 1/13/2015.
 */
public class StartActivity extends Activity{

    private Preview mPreview;
    Camera mCamera;
    int  numberOfCameras;
    int  cameraCurrentlyLocked;
    public Camera.Size mPreviewSize;
    Bitmap previewImageSnapshot;
    Bitmap croppedFaceBitmap;
    Bitmap snapshotImageRGB;
    Bitmap croppedSnapshotImageRGB;
    Bitmap croppedGrayImage;
    Bitmap croppedBackImageRGB;
    Bitmap croppedBackImageThreshold;
    Bitmap croppedBackImageGray;
    private Camera.Face lastFace = null;
    String encodedVals = "";
    int ambientVal = 255;
    int ambientVal1 = 255; // left cheek
    int ambientVal2 = 255; // right cheek
    int ambientVal3 = 255; // left nose
    int ambientVal4 = 255; // mid nose
    int ambientVal5 = 255; // right nose

    int highlight1 = 0; // left cheek
    int highlight2 = 0; // right cheek
    int highlight3 = 0; // left nose
    int highlight4 = 0; // mid nose
    int highlight5 = 0; // right nose

    PreviewImageView iv = null;

    // The first rear facing camera
    int  defaultCameraId;
    public boolean safeToTakePicture = false;
    public boolean safeToDetectFace = false;
	
    private static final Pattern PATTERN = Pattern.compile("[0-9]+_bgw_yaleB[0-9]+_P00A(.{1}[0-9]+)E(.{1}[0-9]+)&bgw_yaleB[0-9]+_P00A(.{1}[0-9]+)E(.{1}[0-9]+).*");
	
    private Handler takePictureHandler;
    public Runnable updateFacePicture = new Runnable()
    {
        public void run()
        {
            if (safeToTakePicture) {
                iv = (PreviewImageView) findViewById(R.id.preview_snapshot);
                iv.setImageBitmap(previewImageSnapshot);
                storeImage(previewImageSnapshot,1);
                snapshotImageRGB = previewImageSnapshot.copy(Bitmap.Config.RGB_565, true);
                if (lastFace != null && encodedVals == "") {

                    // save current rotation and position state in Unity
                    //mUnityPlayer.UnitySendMessage("CamTexture", "saveCurrPosRotState", "1");

                    Rect faceBounds = new Rect(lastFace.rect);
                    faceBounds.bottom = (faceBounds.bottom + 1000) * iv.getHeight() / 2000;
                    faceBounds.top = (faceBounds.top + 1000) * iv.getHeight() / 2000;
                    faceBounds.left = (faceBounds.left + 1000) * iv.getWidth() / 2000;
                    faceBounds.right = (faceBounds.right + 1000) * iv.getWidth() / 2000;
                    iv.faceRect = faceBounds;

                    Rect faceBoundsL = new Rect(lastFace.rect);
                    faceBoundsL.bottom = (faceBoundsL.bottom + 1000) * mPreviewSize.height / 2000;
                    faceBoundsL.top = (faceBoundsL.top + 1000) * mPreviewSize.height / 2000;
                    faceBoundsL.left = (faceBoundsL.left + 1000) * mPreviewSize.width / 2000;
                    faceBoundsL.right = (faceBoundsL.right + 1000) * mPreviewSize.width / 2000;
                    croppedFaceBitmap = Bitmap.createBitmap(previewImageSnapshot,
                            faceBoundsL.left, faceBoundsL.top,
                            faceBoundsL.right - faceBoundsL.left, faceBoundsL.bottom - faceBoundsL.top);
                    croppedSnapshotImageRGB = croppedFaceBitmap.copy(Bitmap.Config.RGB_565, true);
                    croppedBackImageRGB = previewImageSnapshot.copy(Bitmap.Config.RGB_565, true);
                    getBackgroundMask(croppedBackImageRGB, faceBoundsL.left, faceBoundsL.right, faceBoundsL.bottom);

                    croppedBackImageThreshold = croppedBackImageRGB.copy(Bitmap.Config.RGB_565, true);
                    getThresholdImage(croppedBackImageThreshold, 200);
                    iv.setImageBitmap(croppedBackImageThreshold);

                    //storeImage(croppedBackImageRGB,4);
                    //storeImage(croppedBackImageThreshold,5);

                    /*boolean hasBackLight = false;
                    for (int i = 0; i < croppedBackImageThreshold.getWidth(); i++) {
                        for (int j = 0; j < croppedBackImageThreshold.getHeight(); j++) {
                            int pixel = croppedBackImageThreshold.getPixel(i,j);
                            if (Color.red(pixel) > 0 || Color.green(pixel) > 0 || Color.blue(pixel) > 0) {
                                hasBackLight = true;
                                break;
                            }
                        }
                    }
                    if (!hasBackLight) {
                        croppedBackImageThreshold = croppedBackImageRGB.copy(Bitmap.Config.RGB_565, true);
                        croppedBackImageThreshold = getThresholdImage(croppedBackImageThreshold, 100);
                    }*/


                    croppedGrayImage = grayscaleImage(croppedSnapshotImageRGB);
                    storeImage(croppedFaceBitmap,2);
                    detectFace(iv);

                }
                safeToDetectFace = true;
                safeToTakePicture = false;
            }
            if (encodedVals == "") {
                takePictureHandler.postDelayed(updateFacePicture, 3000);
            } else {
                Intent i = new Intent(getApplicationContext(), UnityPlayerNativeActivity.class);
                i.putExtra("vals",encodedVals);
                startActivity(i);
            }
        }
    };

    public int[] getAzimuthElevationFromPattern(String input, Pattern pattern) {
        Matcher m = pattern.matcher(input);

        int[] vals = new int[4];
        if (m.find()) {
            vals[0] = Integer.parseInt(m.group(1)); // az1
            vals[1] = Integer.parseInt(m.group(2)); // el1
            vals[2] = Integer.parseInt(m.group(3)); // az2
            vals[3] = Integer.parseInt(m.group(4)); // el2
        } else {

            Log.w("Regex","No Match!");
        }
        return vals;
    }

    public String getBestMatch(Bitmap image1, String folderName) {
        AssetManager am = getAssets();
        try {
            int bestScore = 0;
            String fileName = "";
            String[] files = am.list(folderName);
            InputStream inputFile = null;
            String file = "";
            for (int i = 0; i < files.length - 1; i++) {
                file = files[i];
                inputFile = getAssets().open("targets/" + file);
                Bitmap image2 = BitmapFactory.decodeStream(inputFile);
                int score = 0;
                for (int x = 0; x < image2.getWidth(); x++) {
                    for (int y = 0; y < image2.getHeight(); y++) {
                        int pixel1 = image1.getPixel(x, y);
                        int pixel2 = image2.getPixel(x, y);
                        int red1 = (pixel1 & 0xff0000) >> 16;
                        int red2 = (pixel2 & 0xff0000) >> 16;
                        if (red1 == red2) {
                            score++;
                        }
                    }
                }
                if (score > bestScore) {
                    bestScore = score;
                    fileName = file;
                }
            }
            Log.w("best match", String.valueOf(bestScore));
            Log.w("best match file", fileName);
        } catch (IOException e) {

        }
        return null;
    }

    private Bitmap scaleImage(Bitmap bitmap, Point centerS, float eyedistS, Point centerT, float eyedistT) {
        float ratio = (float) eyedistT / eyedistS;
        int width = (int) (ratio * bitmap.getWidth());
        int height = (int) (ratio * bitmap.getHeight());
        bitmap = Bitmap.createScaledBitmap(bitmap, width, height, true);
        centerS.x = (int) ( centerS.x * ratio);
        centerS.y = (int) ( centerS.y * ratio);
        int w = 168;
        int h = 192;
        Bitmap transformedBitmap = Bitmap.createBitmap(w, h, Bitmap.Config.RGB_565); // this creates a MUTABLE bitmap
        for (int i = 0; i < transformedBitmap.getWidth(); i++) {
            for (int j = 0; j < transformedBitmap.getHeight(); j++) {
                int newI = centerS.x - centerT.x + i;
                int newJ = centerS.y - centerT.y + j;
                int pixel = bitmap.getPixel(newI, newJ);
                transformedBitmap.setPixel(i,j,pixel);
            }
        }
        Log.w("threshold",String.valueOf(ratio));
        return transformedBitmap;
    }

    private int getOverallBrightnessMean(Bitmap bitmap, Point centerTop, float eyedist) {
        int avgBrightness = 0;
        int sumColor = 0;
        int count = 0;
        for (int i = 0; i < bitmap.getWidth(); i++) {
            for (int j = 0; j < bitmap.getHeight(); j++) {
                int pixel = bitmap.getPixel(i,j);
                sumColor += Color.red(pixel);
                count++;
            }
        }
        avgBrightness = sumColor/count;
        return avgBrightness;
    }

    private Bitmap getFaceLightThresholdMask(Bitmap bitmap, int brightness) {
        Bitmap maskImage = bitmap.copy(Bitmap.Config.RGB_565, true);
        int width = maskImage.getWidth();
        int height = maskImage.getHeight();
        int white = 0xffffff;
        int lgray = 0xcccccc;
        int gray = 0x888888;
        int dgray = 0x444444;
        int black = 0x000000;
        int lowerHalf = brightness/5;
        int upperHalf = (255 - brightness)/5;
        Log.w("brightness", String.valueOf(brightness));
        Log.w("brightness", String.valueOf(lowerHalf));
        Log.w("brightness", String.valueOf(upperHalf));
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int pixel = maskImage.getPixel(i,j);
                int red = Color.red(pixel);
                if (red <= lowerHalf*2) {
                    maskImage.setPixel(i,j,black);
                } else if (red <= lowerHalf*4) {
                    maskImage.setPixel(i,j,dgray);
                } else if (red <= brightness + upperHalf) {
                    maskImage.setPixel(i,j,gray);
                } else if (red <= brightness + upperHalf*3) {
                    maskImage.setPixel(i,j,lgray);
                } else {
                    maskImage.setPixel(i,j,white);
                }
            }
        }
        return maskImage;
    }

    private void getBackgroundMask(Bitmap bitmap, int left, int right, int bottom) {
        for (int i = 0; i < bitmap.getWidth(); i++) {
            for (int j = 0; j < bitmap.getHeight(); j++) {
                if ((i >= (left - (right - left)/5) &&
                        i <= (right - (right - left)/5)) ||
                        j >= bottom) {
                    int maskVal = 0x000000;
                    bitmap.setPixel(i,j, maskVal);
                }
            }
        }
    }

    private void getThresholdImage(Bitmap image, int threshold) {
        int width = image.getWidth();
        int height = image.getHeight();
        int[] pixels = new int[width*height];
        image.getPixels(pixels, 0,width,0,0,width,height);
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                int pixel = pixels[j*width + i];
                int grayVal = (Color.red(pixel) + Color.green(pixel) + Color.blue(pixel))/3;
                if (grayVal > threshold) {
                    image.setPixel(i,j, 0xFFFFFF);
                } else {
                    image.setPixel(i,j, 0x000000);
                }

            }
        }
        Log.w("threshold","got this far");
        //return image;
    }

    private int[] getBackgroundLightColorAndPos(Bitmap imageMask, Bitmap imageRGB) {
        int[] PosCol = new int[6];
        int sumR = 0;
        int sumG = 0;
        int sumB = 0;
        int count = 0;
        int posX = 0;
        int posY = 0;
        for (int i = 0; i < imageMask.getWidth(); i++) {
            for (int j = 0; j < imageMask.getHeight(); j++) {
                int pixel = imageMask.getPixel(i,j);
                if (Color.red(pixel) > 0 || Color.green(pixel) > 0 || Color.blue(pixel) > 0) {
                    int pixelRGB = imageRGB.getPixel(i,j);
                    sumR += Color.red(pixelRGB);
                    sumG += Color.green(pixelRGB);
                    sumB += Color.blue(pixelRGB);
                    count ++;
                    if (posX > 0 || posY > 0) {
                        posX += i;
                        posY += j;
                    } else {
                        posX = i;
                        posY = j;
                    }
                }
            }
        }
        if (count > 0) {
            PosCol[0] = (posX / count);
            PosCol[1] = (posY / count);
            PosCol[2] = sumR / count;
            PosCol[3] = sumG / count;
            PosCol[4] = sumB / count;
            PosCol[5] = count*10;
        } else {
            PosCol[0] = 0;
            PosCol[1] = 0;
            PosCol[2] = 0;
            PosCol[3] = 0;
            PosCol[4] = 0;
            PosCol[5] = 0;
        }
        return PosCol;
    }

    private void detectFace(PreviewImageView iv) {
        FaceDetector fd = new FaceDetector(previewImageSnapshot.getWidth(), previewImageSnapshot.getHeight(), 1);
        FaceDetector.Face[] faces = new FaceDetector.Face[1];
        int count = fd.findFaces(snapshotImageRGB, faces);
        if (count > 0) {
            PointF eyescenter = new PointF();
            float eyedist = 0.0f;
            faces[0].getMidPoint(eyescenter);
            eyedist = faces[0].eyesDistance();

            Point leftEye = new Point();
            Point rightEye = new Point();
            leftEye.x = (int) (eyescenter.x - eyedist / 2);
            leftEye.y = (int) eyescenter.y;
            rightEye.x = (int) (eyescenter.x + eyedist / 2);
            rightEye.y = (int) eyescenter.y;

            Bitmap edgesImage = detectEdges();
            Rect localFace = new Rect(lastFace.rect);
            localFace.bottom = (localFace.bottom + 1000) * mPreviewSize.height / 2000;
            localFace.top = (localFace.top + 1000) * mPreviewSize.height / 2000;
            localFace.left = (localFace.left + 1000) * mPreviewSize.width / 2000;
            localFace.right = (localFace.right + 1000) * mPreviewSize.width / 2000;
            Point localEyesCenter = new Point();
            localEyesCenter.x = (int) (eyescenter.x - localFace.left);
            localEyesCenter.y = (int) (eyescenter.y - localFace.top);
            int sixthDist = (int) (eyedist / 6);
            Point leftNose = new Point();
            leftNose.x = localEyesCenter.x - sixthDist;
            leftNose.y = localEyesCenter.y;
            Point rightNose = new Point();
            rightNose.x = localEyesCenter.x + sixthDist;
            rightNose.y = localEyesCenter.y;
            Point nose = new Point();
            nose.x = localEyesCenter.x;
            nose.y = localEyesCenter.y;
            for (int i = (int) (localEyesCenter.y + eyedist / 2); i < (int) (localEyesCenter.y + eyedist); i++) {
                int pixelL = edgesImage.getPixel(leftNose.x, i);
                int pixelR = edgesImage.getPixel(rightNose.x, i);
                if (Color.red(pixelL) == 255 || Color.red(pixelR) == 255) {
                    nose.x = localEyesCenter.x;
                    nose.y = i;
                    break;
                }
            }
            // get nose region grayscale values
            int[] noseRegionVals = getNoseRegionLuminosity(localEyesCenter, nose, eyedist);
            int[] foreheadRegionVals = getForeHeadLuminosity(localEyesCenter, eyedist);
            int[] cheekRegionVals = getCheekLuminosity(localEyesCenter, eyedist);
            int[] lightColors = getLightColor(localEyesCenter, eyedist);
            int[] ambientVals = new int[2];
            ambientVals[0] = ambientVal1;
            ambientVals[1] = ambientVal2;
            int[] backLight = getBackgroundLightColorAndPos(croppedBackImageThreshold, croppedBackImageRGB);

            Log.w("highlights", String.valueOf(highlight1));
            Log.w("highlights", String.valueOf(highlight2));
            Log.w("highlights", String.valueOf(highlight3));
            Log.w("highlights", String.valueOf(highlight4));
            Log.w("highlights", String.valueOf(highlight5));

            Point targetCenter = new Point(81,41);
            float targetEyeDist = 94f;
            Bitmap scaledToRef = scaleImage(croppedGrayImage,localEyesCenter,eyedist,targetCenter,targetEyeDist);
            int overallBrightnessMean = getOverallBrightnessMean(scaledToRef,localEyesCenter,eyedist);
            Bitmap brightnessMask = getFaceLightThresholdMask(scaledToRef,overallBrightnessMean);
            String bestMatchName = getBestMatch(scaledToRef, "targets/");
            iv.setImageBitmap(brightnessMask);
            storeImage(brightnessMask,4);

            int[] azimuthElevationVals = getAzimuthElevationFromPattern(bestMatchName, PATTERN);

            encodedVals = encodeValues(noseRegionVals, foreheadRegionVals, cheekRegionVals,
                    lightColors, ambientVal, ambientVals, backLight, azimuthElevationVals);

            //mUnityPlayer.UnitySendMessage("CamTexture", "receiveLuminosityVals", encodedVals);

            // convert nose to small preview frame
            nose.x = nose.x + localFace.left;
            nose.y = nose.y + localFace.top;
            nose.x = nose.x * iv.getWidth() / previewImageSnapshot.getWidth();
            nose.y = nose.y * iv.getHeight()/ previewImageSnapshot.getHeight();

            // convert eyes to small preview frame
            leftEye.x = leftEye.x * iv.getWidth() / previewImageSnapshot.getWidth();
            leftEye.y = leftEye.y * iv.getHeight()/ previewImageSnapshot.getHeight();
            rightEye.x = rightEye.x * iv.getWidth() / previewImageSnapshot.getWidth();
            rightEye.y = rightEye.y * iv.getHeight()/ previewImageSnapshot.getHeight();

            // display nose and eyes on small preview
            iv.leftEye = leftEye;
            iv.rightEye = rightEye;
            iv.nose = nose;
            iv.invalidate();
            iv.drawRect = true;
        }
    }

    private String encodeValues(
            int[] nose, int[] forehead, int[] cheeks, int[] lightCols,
            int ambient, int[] ambVals, int[] backLight, int[] azel) {
        String out = "";
        for (int i = 0; i < nose.length; i++) {
            out += nose[i];
            out += ",";
        }
        for (int i = 0; i < forehead.length; i++) {
            out += forehead[i];
            out += ",";
        }
        for (int i = 0; i < cheeks.length; i++) {
            out += cheeks[i];
            out += ",";
        }
        for (int i = 0; i < lightCols.length; i++) {
            out += lightCols[i];
            out += ",";
        }
        out += ambient;
        out += ",";
        for (int i = 0; i < ambVals.length; i++) {
            out += ambVals[i];
            out += ",";
        }
        for (int i = 0; i < backLight.length; i++) {
            out += backLight[i];
            out += ",";
        }
        for (int i = 0; i < azel.length; i++) {
            out += azel[i];
            out += ",";
        }
        out = out.substring(0,out.length()-1);
        return out;
    }

    private int[] getLightColor(Point centerTop, float eyedist) {
        int[] colors = new int[6];
        int halfDist = (int) eyedist / 2;
        int quarterDist = (int) eyedist / 4;
        Point centerMid = new Point();
        centerMid.x = centerTop.x;
        centerMid.y = centerTop.y + quarterDist;
        int leftLeft = centerMid.x - quarterDist - halfDist;
        int leftRight = centerMid.x - quarterDist;
        int rightLeft = centerMid.x + quarterDist;
        int rightRight = centerMid.x + quarterDist + halfDist;
        int top = centerMid.y;
        int bot = centerMid.y + halfDist;
        // region 1 value
        int R1RSum = 0;
        int R1GSum = 0;
        int R1BSum = 0;
        int R1Count = 0;
        for (int i = leftLeft; i < leftRight; i++) {
            for (int j = top; j < bot; j++) {
                int r = Color.red(croppedSnapshotImageRGB.getPixel(i, j));
                int g = Color.green(croppedSnapshotImageRGB.getPixel(i, j));
                int b = Color.blue(croppedSnapshotImageRGB.getPixel(i, j));
                R1RSum += r;
                R1GSum += g;
                R1BSum += b;
                R1Count++;
            }
        }
        int R1RAvg = (int) R1RSum / R1Count;
        int R1GAvg = (int) R1GSum / R1Count;
        int R1BAvg = (int) R1BSum / R1Count;
        float rf = (float) R1RAvg / 95.0f * 4.0f;
        float gf = (float) R1GAvg / 40.0f * 2.0f;
        float bf = (float) R1BAvg / 20.0f * 1.0f;
        float maxV = Math.max(rf,gf); maxV = Math.max(maxV, bf);
        int maxVint = (int) maxV + 1;
        rf = (float) rf / maxVint;
        gf = (float) gf / maxVint;
        bf = (float) bf / maxVint;
        rf = rf * 255.0f;
        gf = gf * 255.0f;
        bf = bf * 255.0f;
        int rC = (int) rf;
        int gC = (int) gf;
        int bC = (int) bf;
        maxVint = Math.max(rC,gC); maxVint = Math.max(maxVint, bC);
        //rC = 255 - maxVint + rC;
        //gC = 255 - maxVint + gC;
        //bC = 255 - maxVint + bC;
        colors[0] = rC;
        colors[1] = gC;
        colors[2] = bC;
        // region 2 value
        int R2RSum = 0;
        int R2GSum = 0;
        int R2BSum = 0;
        int R2Count = 0;
        for (int i = rightLeft; i < rightRight; i++) {
            for (int j = top; j < bot; j++) {
                int r = Color.red(croppedSnapshotImageRGB.getPixel(i, j));
                int g = Color.green(croppedSnapshotImageRGB.getPixel(i, j));
                int b = Color.blue(croppedSnapshotImageRGB.getPixel(i, j));
                R2RSum += r;
                R2GSum += g;
                R2BSum += b;
                R2Count++;
            }
        }
        int R2RAvg = (int) R2RSum / R2Count;
        int R2GAvg = (int) R2GSum / R2Count;
        int R2BAvg = (int) R2BSum / R2Count;
        rf = (float) R2RAvg / 95.0f * 4.0f;
        gf = (float) R2GAvg / 40.0f * 2.0f;
        bf = (float) R2BAvg / 20.0f * 1.0f;
        maxV = Math.max(rf,gf); maxV = Math.max(maxV, bf);
        maxVint = (int) maxV + 1;
        rf = (float) rf / maxVint;
        gf = (float) gf / maxVint;
        bf = (float) bf / maxVint;
        rf = rf * 255.0f;
        gf = gf * 255.0f;
        bf = bf * 255.0f;
        rC = (int) rf;
        gC = (int) gf;
        bC = (int) bf;
        maxVint = Math.max(rC,gC); maxVint = Math.max(maxVint, bC);
        //rC = 255 - maxVint + rC;
        //gC = 255 - maxVint + gC;
        //bC = 255 - maxVint + bC;
        colors[3] = rC;
        colors[4] = gC;
        colors[5] = bC;
        return colors;
    }

    private int[] getCheekLuminosity(Point centerTop, float eyedist) {
        int halfDist = (int) eyedist / 2;
        int quarterDist = (int) eyedist / 4;
        Point centerMid = new Point();
        centerMid.x = centerTop.x;
        centerMid.y = centerTop.y + quarterDist;
        int leftLeft = centerMid.x - quarterDist - halfDist;
        int leftRight = centerMid.x - quarterDist;
        int rightLeft = centerMid.x + quarterDist;
        int rightRight = centerMid.x + quarterDist + halfDist;
        int top = centerMid.y;
        int bot = centerMid.y + halfDist;
        // region 1 value
        int R1Sum = 0;
        int R1Count = 0;
        for (int i = leftLeft; i < leftRight; i++) {
            for (int j = top; j < bot; j++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                if (gray < ambientVal1) ambientVal1 = gray;
                if (gray > highlight1) highlight1 = gray;
                R1Sum += gray;
                R1Count++;
            }
        }
        int R1Avg = (int) R1Sum / R1Count;
        // region 2 value
        int R2Sum = 0;
        int R2Count = 0;
        for (int i = rightLeft; i < rightRight; i++) {
            for (int j = top; j < bot; j++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                if (gray < ambientVal2) ambientVal2 = gray;
                if (gray > highlight2) highlight2 = gray;
                R2Sum += gray;
                R2Count++;
            }
        }
        int R2Avg = (int) R2Sum / R2Count;
        int[] vals = new int[2];
        vals[0] = R1Avg;
        vals[1] = R2Avg;
        Log.w("ambient", String.valueOf(ambientVal));
        Log.w("ambient", String.valueOf(ambientVal1));
        Log.w("ambient", String.valueOf(ambientVal2));
        return vals;
    }

    private int[] getForeHeadLuminosity(Point center, float eyedist) {
        int quarterDist = (int) eyedist / 4;
        int halfDist = (int) eyedist / 2;
        int left = center.x - quarterDist;
        int top = center.y - halfDist;
        int bot = center.y;
        int right = center.x + quarterDist;
        int mid = center.y - quarterDist;
        // region 1 value
        int R1Sum = 0;
        int R1Count = 0;
        for (int i = left; i < right; i++) {
            for (int j = mid; j < bot; j++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                R1Sum += gray;
                R1Count++;
            }
        }
        int R1Avg = (int) R1Sum / R1Count;
        // region 2 value
        int R2Sum = 0;
        int R2Count = 0;
        for (int i = left; i < right; i++) {
            for (int j = top; j < mid; j++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                R2Sum += gray;
                R2Count++;
            }
        }
        int R2Avg = (int) R2Sum / R2Count;
        int[] vals = new int[2];
        vals[0] = R1Avg;
        vals[1] = R2Avg;
        return vals;
    }

    private int[] getNoseRegionLuminosity(Point centerTop, Point centerBot, float eyedist) {
        int quarterDist = (int) eyedist / 4;
        Point leftNose = new Point();
        Point rightNose = new Point();
        leftNose.x = centerBot.x - quarterDist;
        leftNose.y = centerBot.y;
        rightNose.x = centerBot.x + quarterDist;
        rightNose.y = centerBot.y;
        float slope = (float) (centerBot.y - centerTop.y) / (centerBot.x - leftNose.x);
        //left nose side
        int leftValSum = 0;
        int leftCount = 0;
        for (int j = centerTop.y; j < centerBot.y; j++) {
            for (int i = (int) ((int)centerBot.x - ((int) (j - centerTop.y)/slope)); i < centerBot.x; i++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                if (gray > highlight3) highlight3 = gray;
                leftValSum += gray;
                leftCount++;
            }
        }
        int leftValAvg = leftValSum / leftCount;
        //right nose side
        int rightValSum = 0;
        int rightCount = 0;
        for (int j = centerTop.y; j < centerBot.y; j++) {
            for (int i = centerBot.x; i < (int) ((int)centerBot.x + ((int) (j - centerTop.y)/slope)); i++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                if (gray > highlight5) highlight5 = gray;
                rightValSum += gray;
                rightCount++;
            }
        }
        int rightValAvg = rightValSum / rightCount;
        //middle nose line
        int midValSum = 0;
        int midCount = 0;
        for (int i = centerTop.x - quarterDist/4; i < centerTop.x + quarterDist/4; i++) {
            for (int j = centerTop.y; j < centerBot.y; j++) {
                int gray = Color.blue(croppedGrayImage.getPixel(i, j));
                if (gray < ambientVal) ambientVal = gray;
                if (gray > highlight4) highlight4 = gray;
                midValSum += gray;
                midCount++;
            }
        }
        int midValAvg = midValSum / midCount;
        int[] vals = new int[3];
        vals[0] = leftValAvg;
        vals[1] = midValAvg;
        vals[2] = rightValAvg;
        return vals;
    }

    private Bitmap grayscaleImage(Bitmap image) {
        Bitmap grayImage = image.copy(image.getConfig(), true);
        for (int i = 0; i < image.getWidth(); i++) {
            for (int j = 0; j < image.getHeight(); j++) {
                int pixel = grayImage.getPixel(i,j);
                int grayVal = (Color.red(pixel) + Color.green(pixel) + Color.blue(pixel))/3;
                grayImage.setPixel(i,j, Color.rgb(grayVal, grayVal, grayVal));
            }
        }
        return grayImage;
    }

    private Bitmap localMean() {
        Bitmap image = croppedGrayImage.copy(Bitmap.Config.RGB_565, true);
        for (int i = 0; i < image.getWidth(); i++) {
            for (int j = 0; j < image.getHeight(); j++) {

            }
        }
        return image;
    }

    private Bitmap detectEdges() {
        CannyEdgeDetector ced = new CannyEdgeDetector();
        ced.setSourceImg(croppedSnapshotImageRGB);
        ced.lowThreshold = 1.0f;
        ced.highThreshold = 2.5f;
        ced.process();
        Bitmap edges = ced.getEdgesImg();
        storeImage(edges,3);
        return edges;
    }

    private void storeImage(Bitmap image, int id) {
        File pictureFile = getOutputMediaFile(id);
        if (pictureFile == null) {
            Log.d("I/O",
                    "Error creating media file, check storage permissions: ");// e.getMessage());
            return;
        }
        try {
            FileOutputStream fos = new FileOutputStream(pictureFile);
            image.compress(Bitmap.CompressFormat.PNG, 90, fos);
            fos.close();
        } catch (FileNotFoundException e) {
            Log.d("I/O", "File not found: " + e.getMessage());
        } catch (IOException e) {
            Log.d("I/O", "Error accessing file: " + e.getMessage());
        }
    }

    private  File getOutputMediaFile(int id){
        // To be safe, you should check that the SDCard is mounted
        // using Environment.getExternalStorageState() before doing this.
        File mediaStorageDir = new File(Environment.getExternalStorageDirectory()
                + "/Android/data/"
                + getApplicationContext().getPackageName()
                + "/files");

        // This location works best if you want the created images to be shared
        // between applications and persist after your app has been uninstalled.

        // Create the storage directory if it does not exist
        if (! mediaStorageDir.exists()){
            if (! mediaStorageDir.mkdirs()){
                return null;
            }
        }
        // Create a media file name
        String timeStamp = new SimpleDateFormat("ddMMyyyy_HHmm").format(new Date());
        File mediaFile;
        String mImageName="MI_"+ timeStamp + "_" + String.valueOf(id) + ".png";
        mediaFile = new File(mediaStorageDir.getPath() + File.separator + mImageName);
        return mediaFile;
    }

    @Override protected void onCreate (Bundle savedInstanceState)
    {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        // Hide the window title.
        //requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);

        // Create a RelativeLayout container that will hold a SurfaceView,
        // and set it as the content of our activity.
        mPreview = new  Preview(this);
        //setContentView(mPreview);

        // Find the total number of cameras available
        numberOfCameras = Camera.getNumberOfCameras();

        // Find the ID of the default camera
        defaultCameraId = 1;

        setContentView(R.layout.main_layout);

        takePictureHandler = new Handler();
        takePictureHandler.postDelayed(updateFacePicture, 5000);

        getWindow().takeSurface(null);
        //setTheme(android.R.style.Theme_NoTitleBar_Fullscreen);
        //getWindow().setFormat(PixelFormat.RGB_565);

        FrameLayout back_camera = (FrameLayout) findViewById(R.id.back_camera);
        back_camera.addView(mPreview);

        //mUnityPlayer.requestFocus();

        // -- TEST message test
        //mUnityPlayer.UnitySendMessage("CamTexture", "receiveMessage", "test message sent");
    }

    @Override protected void onDestroy ()
    {
        super.onDestroy();
    }

    // Pause Unity
    @Override protected void onPause()
    {
        super.onPause();

        // Because the Camera object is a shared resource, it's very
        // important to release it when the activity is paused.
        if  (mCamera != null) {
            mPreview.setCamera(null);
            mCamera.setPreviewCallback(null);
            mCamera.release();
            mCamera = null;
        }
    }

    // Resume Unity
    @Override protected void onResume()
    {
        super.onResume();

        // Open the default i.e. the first rear facing camera.
        // TODO: experiment with this, causing crashes
        mCamera = Camera.open(2);
        // Camera face detection in preview
        mCamera.setFaceDetectionListener(new Camera.FaceDetectionListener() {
            @Override
            public void onFaceDetection(Camera.Face[] faces, Camera camera) {
                if (faces.length > 0 && safeToDetectFace) {
                    Camera.Face f = faces[0];
                    lastFace = f;
                    safeToDetectFace = false;
                }
            }
        });
        cameraCurrentlyLocked = defaultCameraId;
        if (mCamera != null) {
            mPreview.setCamera(mCamera);
        }
    }

    // This ensures the layout will be correct.
    @Override public void onConfigurationChanged(Configuration newConfig)
    {
        super.onConfigurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    @Override public void onWindowFocusChanged(boolean hasFocus)
    {
        super.onWindowFocusChanged(hasFocus);
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    @Override public boolean dispatchKeyEvent(KeyEvent event)
    {
        return super.dispatchKeyEvent(event);
    }


    class  Preview extends ViewGroup implements  SurfaceHolder.Callback {
        private  final  String TAG = "Preview";

        SurfaceView mSurfaceView;
        SurfaceHolder mHolder;
        List<Camera.Size> mSupportedPreviewSizes;
        Camera mCamera;

        Preview(Context context) {
            super(context);

            mSurfaceView = new  SurfaceView(context);
            addView(mSurfaceView);

            // Install a SurfaceHolder.Callback so we get notified when the
            // underlying surface is created and destroyed.
            mHolder = mSurfaceView.getHolder();
            mHolder.addCallback(this);
            mHolder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
        }

        public  void  setCamera(Camera camera) {
            mCamera = camera;
            if  (mCamera != null) {
                mSupportedPreviewSizes = mCamera.getParameters().getSupportedPreviewSizes();
                requestLayout();
            }
        }

        public  void  switchCamera(Camera camera) {
            setCamera(camera);
            try  {
                camera.setPreviewDisplay(mHolder);
            } catch  (IOException exception) {
                Log.e(TAG, "IOException caused by setPreviewDisplay()", exception);
            }
            Camera.Parameters parameters = camera.getParameters();
            parameters.setPreviewSize(mPreviewSize.width, mPreviewSize.height);
            requestLayout();

            camera.setParameters(parameters);
        }

        @Override
        protected  void  onMeasure(int  widthMeasureSpec, int  heightMeasureSpec) {
            // We purposely disregard child measurements because act as a
            // wrapper to a SurfaceView that centers the camera preview instead
            // of stretching it.
            final  int  width = resolveSize(getSuggestedMinimumWidth(), widthMeasureSpec);
            final  int  height = resolveSize(getSuggestedMinimumHeight(), heightMeasureSpec);
            setMeasuredDimension(width, height);

            if  (mSupportedPreviewSizes != null) {
                mPreviewSize = getOptimalPreviewSize(mSupportedPreviewSizes, width, height);
            }
        }

        @Override
        protected  void  onLayout(boolean  changed, int  l, int  t, int  r, int  b) {
            if  (changed && getChildCount() > 0) {
                final View child = getChildAt(0);

                final  int  width = r - l;
                final  int  height = b - t;

                int  previewWidth = width;
                int  previewHeight = height;
                if  (mPreviewSize != null) {
                    previewWidth = mPreviewSize.width;
                    previewHeight = mPreviewSize.height;
                }

                // Center the child SurfaceView within the parent.
                if  (width * previewHeight > height * previewWidth) {
                    final  int  scaledChildWidth = previewWidth * height / previewHeight;
                    child.layout((width - scaledChildWidth) / 2, 0,
                            (width + scaledChildWidth) / 2, height);
                } else  {
                    final  int  scaledChildHeight = previewHeight * width / previewWidth;
                    child.layout(0, (height - scaledChildHeight) / 2,
                            width, (height + scaledChildHeight) / 2);
                }
            }
        }

        public  void  surfaceCreated(SurfaceHolder holder) {
            // The Surface has been created, acquire the camera and tell it where
            // to draw.
            try  {
                if  (mCamera != null) {
                    mCamera.setPreviewDisplay(holder);
                }
            } catch  (IOException exception) {
                Log.e(TAG, "IOException caused by setPreviewDisplay()", exception);
            }
        }

        public  void  surfaceDestroyed(SurfaceHolder holder) {
            // Surface will be destroyed when we return, so stop the preview.
            if  (mCamera != null) {
                mCamera.stopPreview();
                mCamera.setPreviewCallback(null);
            }
        }


        private Camera.Size getOptimalPreviewSize(List<Camera.Size> sizes, int  w, int  h) {
            final  double  ASPECT_TOLERANCE = 0.1;
            double  targetRatio = (double) w / h;
            if  (sizes == null) return  null;

            Camera.Size optimalSize = null;
            double  minDiff = Double.MAX_VALUE;

            int  targetHeight = h;

            // Try to find an size match aspect ratio and size
            for  (Camera.Size size : sizes) {
                double  ratio = (double) size.width / size.height;
                if  (Math.abs(ratio - targetRatio) > ASPECT_TOLERANCE) continue;
                if  (Math.abs(size.height - targetHeight) < minDiff) {
                    optimalSize = size;
                    minDiff = Math.abs(size.height - targetHeight);
                }
            }

            // Cannot find the one match the aspect ratio, ignore the requirement
            if  (optimalSize == null) {
                minDiff = Double.MAX_VALUE;
                for  (Camera.Size size : sizes) {
                    if  (Math.abs(size.height - targetHeight) < minDiff) {
                        optimalSize = size;
                        minDiff = Math.abs(size.height - targetHeight);
                    }
                }
            }
            return  optimalSize;
        }

        public  void  surfaceChanged(SurfaceHolder holder, int  format, int  w, int  h) {
            // Now that the size is known, set up the camera parameters and begin
            // the preview.
            if (mCamera != null) {
                Camera.Parameters parameters = mCamera.getParameters();
                parameters.setPreviewSize(mPreviewSize.width, mPreviewSize.height);
                parameters.setPreviewFormat(ImageFormat.NV21);
                requestLayout();

                final int[] testCount = {0};

                mCamera.setParameters(parameters);
                mCamera.startPreview();
                mCamera.startFaceDetection();

                mCamera.setPreviewCallback(new Camera.PreviewCallback() {
                    @Override
                    public void onPreviewFrame(byte[] data, Camera camera) {
                        if (!safeToTakePicture) {
                            safeToTakePicture = true;
                            testCount[0]++;
                            Log.w("format", "preview callback");
                            Toast.makeText(getApplicationContext(), "picture taken " + testCount[0],
                                    Toast.LENGTH_SHORT).show();
                            ByteArrayOutputStream out = new ByteArrayOutputStream();
                            YuvImage yuvImage = new YuvImage(data, ImageFormat.NV21, mPreviewSize.width, mPreviewSize.height, null);
                            yuvImage.compressToJpeg(new Rect(0, 0, mPreviewSize.width, mPreviewSize.height), 50, out);
                            byte[] imageBytes = out.toByteArray();
                            previewImageSnapshot = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.length);
                        }
                    }
                });
            }
        }

    }
}
