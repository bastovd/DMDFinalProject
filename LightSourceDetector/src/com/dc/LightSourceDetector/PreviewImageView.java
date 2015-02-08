package com.dc.LightSourceDetector;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Point;
import android.graphics.Rect;
import android.util.AttributeSet;
import android.widget.ImageView;

/**
 * Created by Anton on 1/7/2015.
 */
public class PreviewImageView extends ImageView {
    private Paint currentPaint;
    public boolean drawRect = false;
    public float left;
    public float top;
    public float right;
    public float bottom;
    public Rect faceRect;
    public Point leftEye;
    public Point rightEye;
    public Point nose;

    public PreviewImageView(Context context, AttributeSet attrs) {
        super(context, attrs);

        currentPaint = new Paint();
        currentPaint.setDither(true);
        currentPaint.setColor(0xFF00CC00);  // alpha.r.g.b
        currentPaint.setStyle(Paint.Style.STROKE);
        currentPaint.setStrokeJoin(Paint.Join.ROUND);
        currentPaint.setStrokeCap(Paint.Cap.ROUND);
        currentPaint.setStrokeWidth(2);
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);
        if (drawRect)
        {
            canvas.drawRect(faceRect, currentPaint);
            canvas.drawPoint(leftEye.x, leftEye.y, currentPaint);
            canvas.drawPoint(rightEye.x, rightEye.y, currentPaint);
            canvas.drawPoint(nose.x, nose.y, currentPaint);
        }
    }
}
