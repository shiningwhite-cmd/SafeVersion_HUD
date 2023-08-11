# import the necessary packages
from imutils.video import VideoStream
import argparse
import imutils
import time
import sys
import asyncio
import logging
import os
import numpy as np
import cv2
import dotenv

from g3pylib import connect_to_glasses

logging.basicConfig(level=logging.INFO)

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument(
    "-t", "--type", type=str, default="DICT_4X4_50", help="type of ArUCo tag to detect"
)
args = vars(ap.parse_args())
# define names of each possible ArUco tag OpenCV supports
ARUCO_DICT = {
    "DICT_4X4_50": cv2.aruco.DICT_4X4_50,
    "DICT_4X4_100": cv2.aruco.DICT_4X4_100,
    "DICT_4X4_250": cv2.aruco.DICT_4X4_250,
    "DICT_4X4_1000": cv2.aruco.DICT_4X4_1000,
    "DICT_5X5_50": cv2.aruco.DICT_5X5_50,
    "DICT_5X5_100": cv2.aruco.DICT_5X5_100,
    "DICT_5X5_250": cv2.aruco.DICT_5X5_250,
    "DICT_5X5_1000": cv2.aruco.DICT_5X5_1000,
    "DICT_6X6_50": cv2.aruco.DICT_6X6_50,
    "DICT_6X6_100": cv2.aruco.DICT_6X6_100,
    "DICT_6X6_250": cv2.aruco.DICT_6X6_250,
    "DICT_6X6_1000": cv2.aruco.DICT_6X6_1000,
    "DICT_7X7_50": cv2.aruco.DICT_7X7_50,
    "DICT_7X7_100": cv2.aruco.DICT_7X7_100,
    "DICT_7X7_250": cv2.aruco.DICT_7X7_250,
    "DICT_7X7_1000": cv2.aruco.DICT_7X7_1000,
    "DICT_ARUCO_ORIGINAL": cv2.aruco.DICT_ARUCO_ORIGINAL,
    # 	"DICT_APRILTAG_16h5": cv2.aruco.DICT_APRILTAG_16h5,
    # 	"DICT_APRILTAG_25h9": cv2.aruco.DICT_APRILTAG_25h9,
    # 	"DICT_APRILTAG_36h10": cv2.aruco.DICT_APRILTAG_36h10,
    # 	"DICT_APRILTAG_36h11": cv2.aruco.DICT_APRILTAG_36h11
}
# mtx&dist
mtx = np.array(
    [
        [1.19993253e03, 0.00000000e00, 1.03804779e03],
        [0.00000000e00, 1.19162995e03, 4.60080286e02],
        [0.00000000e00, 0.00000000e00, 1.00000000e00],
    ]
)
dist = np.array([0.01974008, 0.17841985, 0.00847621, 0.05810671, -0.10942774])
rawPts = np.empty((4, 2), dtype=np.float32)
goalPts = np.array([[0, 0], [1920, 0], [1920, 1080], [0, 1080]], dtype=np.float32)

# verify that the supplied ArUCo tag exists and is supported by
# OpenCV
if ARUCO_DICT.get(args["type"], None) is None:
    print("[INFO] ArUCo tag of '{}' is not supported".format(args["type"]))
    sys.exit(0)


# load the ArUCo dictionary and grab the ArUCo parameters
print("[INFO] detecting '{}' tags...".format(args["type"]))
arucoDict = cv2.aruco.Dictionary_get(ARUCO_DICT[args["type"]])
arucoParams = cv2.aruco.DetectorParameters_create()


async def stream_rtsp():
    async with connect_to_glasses.with_hostname(
        os.environ["G3_HOSTNAME"], using_zeroconf=True
    ) as g3:
        async with g3.stream_rtsp(scene_camera=True, gaze=True) as streams:
            async with streams.gaze.decode() as gaze_stream, streams.scene_camera.decode() as scene_stream:
                for i in range(500):
                    frame, frame_timestamp = await scene_stream.get()
                    gaze, gaze_timestamp = await gaze_stream.get()
                    while gaze_timestamp is None or frame_timestamp is None:
                        if frame_timestamp is None:
                            frame, frame_timestamp = await scene_stream.get()
                        if gaze_timestamp is None:
                            gaze, gaze_timestamp = await gaze_stream.get()
                    while gaze_timestamp < frame_timestamp:
                        gaze, gaze_timestamp = await gaze_stream.get()
                        while gaze_timestamp is None:
                            gaze, gaze_timestamp = await gaze_stream.get()

                    logging.info(f"Frame timestamp: {frame_timestamp}")
                    logging.info(f"Gaze timestamp: {gaze_timestamp}")
                    frame = frame.to_ndarray(format="bgr24")

                    # detect ArUco markers in the input frame
                    (corners, ids, rejected) = cv2.aruco.detectMarkers(
                        frame, arucoDict, parameters=arucoParams
                    )

                    # verify *at least* one ArUco marker was detected
                    if len(corners) > 0:
                        # flatten the ArUco IDs list
                        ids = ids.flatten()
                        # otherwise, we've found the four ArUco markers, so we can continue
                        # by flattening the ArUco IDs list and initializing our list of
                        # reference points
                        print("[INFO] constructing augmented reality visualization...")

                        # if we have not found four markers in the input image then we cannot
                        # apply our augmented reality technique
                        if len(corners) != 4:
                            print("[INFO] could not find 4 corners...exiting")
                        else:
                            # loop over the IDs of the ArUco markers in top-left, top-right,
                            # bottom-right, and bottom-left order
                            for i in (1, 2, 3, 4):
                                # grab the index of the corner with the current ID and append the
                                # corner (x, y)-coordinates to our list of reference points
                                j = np.squeeze(np.where(ids == i))
                                corner = np.squeeze(corners[j])
                                rawPts.append(corner)
                    # print(rawPts.shape)
                    # print(goalPts.shape)
                    M = cv2.getPerspectiveTransform(rawPts, goalPts)

                    # If given gaze data
                    if "gaze2d" in gaze:
                        gaze2d = gaze["gaze2d"]
                        logging.info(f"Gaze2d: {gaze2d[0]:9.4f},{gaze2d[1]:9.4f}")

                        # Convert rational (x,y) to pixel location (x,y)
                        h, w = frame.shape[:2]
                        fix = (int(gaze2d[0] * w), int(gaze2d[1] * h))

                        # Draw gaze
                        frame = cv2.circle(frame, fix, 10, (0, 0, 255), 3)
                        if M is not None:
                            u = fix[0]
                            v = fix[1]
                            x = (M[0][0] * u + M[0][1] * v + M[0][2]) / (
                                M[2][0] * u + M[2][1] * v + M[2][2]
                            )
                            y = (M[1][0] * u + M[1][1] * v + M[1][2]) / (
                                M[2][0] * u + M[2][1] * v + M[2][2]
                            )
                            # PRINT DOT
                            print("Transformed Point: ({}, {})".format(x, y))
                        else:
                            print("M is None")

                    elif i % 50 == 0:
                        logging.info(
                            "No gaze data received. Have you tried putting on the glasses?"
                        )

                    cv2.imshow("Video", frame)  # type: ignore
                    if cv2.waitKey(1) & 0xFF == ord("q"):  # 按下 'q' 键退出
                        break


def main():
    asyncio.run(stream_rtsp())
    cv2.destroyAllWindows()


if __name__ == "__main__":
    dotenv.load_dotenv()
    main()
