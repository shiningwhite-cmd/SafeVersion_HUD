# import the necessary packages
# from imutils.video import VideoStream
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
import socket
import time
import sys
import select
import random

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
                # 创建服务器
                server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                server_address = ("localhost", 12345)
                server_socket.bind(server_address)
                server_socket.listen(1)
                print("等待Unity连接...")
                # # 等待连接
                unity_socket, client_address = server_socket.accept()
                print("Unity已连接")
                while(True):
                    ################################
                    ### 时间戳同步算法 ##############
                    ################################
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
                    # logging.info(f"Frame timestamp: {frame_timestamp}")
                    # logging.info(f"Gaze timestamp: {gaze_timestamp}")
                    frame = frame.to_ndarray(format="bgr24")

                    # detect ArUco markers in the input frame
                    (corners, ids, rejected) = cv2.aruco.detectMarkers(
                        frame, arucoDict, parameters=arucoParams
                    )

                    # verify *at least* one ArUco marker was detected
                    if len(corners) > 0:
                        # loop over the detected ArUCo corners
                        # if we have not found four markers in the input image then we cannot
                        # apply our augmented reality technique
                        if len(corners) != 4:
                            print("[INFO] could not find 4 corners...exiting")
                            continue
                        # otherwise,ve found the four ArUco markers, so we can continue
                        # by flattening the ArUco IDs list and initializing our list of
                        # reference points
                        print("[INFO] 4 markers")
                        ids = ids.flatten()
                        refPts = []

                        print("strat")

                        # # loop over the IDs of the ArUco markers in top-left, top-right,
                        # # bottom-right, and bottom-left order
                        # for i in (1, 2, 3, 4):
                        #     # grab the index of the corner with the current ID and append the
                        #     # corner (x, y)-coordinates to our list of reference points
                        #     # rearrange, from raw ids list to ranked list
                        #     j = np.squeeze(np.where(ids == i))
                        #     print(j)
                        #     print(type(j))
                        #     # j = int(j[0])
                        #     # corner = np.squeeze(np.array(corners)[j])
                        #     corner = np.squeeze(corners[j])
                        #     refPts.append(corner)
                        #     print(refPts)
                        #     # unpack our ArUco reference points and use the reference points to
                        #     # define the *destination* transform matrix, making sure the points
                        #     # are specified in top-left, top-right, bottom-right, and bottom-left order
                        
                        ##############################
                        ### 不确定的巨大魔改 ##########
                        ##############################
                        corner = np.squeeze(corners[3])
                        refPts.append(corner)
                        corner = np.squeeze(corners[2])
                        refPts.append(corner)
                        corner = np.squeeze(corners[0])
                        refPts.append(corner)
                        corner = np.squeeze(corners[1])
                        refPts.append(corner)
                        (refPtTL, refPtTR, refPtBR, refPtBL) = refPts
                        dstMat = np.array(
                            [
                                refPtTL[0],
                                refPtTR[1],
                                refPtBR[2],
                                refPtBL[3],
                            ]
                        )
                        # print("refPts=")
                        # print(refPts)
                        # grab the spatial dimensions of the source image and define the
                        # transform matrix for the *source* image in top-left, top-right,
                        # bottom-right, and bottom-left order
                        # print("dstMat=")
                        # print(dstMat)
                        srcMat = np.array([[0.0, 0.0], [1920.0, 0.0], [1920.0, 1080.0], [0.0, 1080.0]])  
                        srcMat = srcMat.astype(np.float32)
                        # print("srcMat=")
                        # print(srcMat)
                        # 求变换矩阵M，已知原图img一像素点坐标p(x,y)，变换前矩阵坐标pts1，变换后矩阵坐标pts2，求变换后p点对应坐标
                        H = cv2.getPerspectiveTransform(dstMat, srcMat)

                        # 坐标转换
                        def cvt_pos(pos, cvt_mat_t):
                            u = pos[0]
                            v = pos[1]
                            x = (
                                cvt_mat_t[0][0] * u
                                + cvt_mat_t[0][1] * v
                                + cvt_mat_t[0][2]
                            ) / (
                                cvt_mat_t[2][0] * u
                                + cvt_mat_t[2][1] * v
                                + cvt_mat_t[2][2]
                            )
                            y = (
                                cvt_mat_t[1][0] * u
                                + cvt_mat_t[1][1] * v
                                + cvt_mat_t[1][2]
                            ) / (
                                cvt_mat_t[2][0] * u
                                + cvt_mat_t[2][1] * v
                                + cvt_mat_t[2][2]
                            )
                            return (int(x), int(y))

                        # If given gaze data
                        if "gaze2d" in gaze:
                            gaze2d = gaze["gaze2d"]  # gaze2d<class 'list'>
                            # <class 'list'>
                            # INFO:root:Gaze2d:    0.5326,   0.3335

                            # logging.info(f"Gaze2d: {gaze2d[0]:9.4f},{gaze2d[1]:9.4f}")

                            # Convert rational (x,y) to pixel location (x,y)
                            h, w = frame.shape[:2]
                            fix = (int(gaze2d[0] * w), int(gaze2d[1] * h))
                            # print(fix)

                            # Draw gaze
                            frame = cv2.circle(frame, fix, 10, (0, 0, 255), 3)
                            # Perspective transformation
                            src_point = np.array(
                                [int(gaze2d[0] * w), int(gaze2d[1] * h)]
                            )
                            # print("src_point=" + str(src_point))
                            squeezed_refPtBl_3 = np.squeeze(refPtBL[3])
                            # if(src_point[0]<refPts[0][0][0] or src_point[0]>refPts[1][1][0] or src_point[1]<refPts[0][0][1] or src_point[1]>refPts[3][2][1]):
                            #     print("out of range")
                            ##########################################
                            ### 这块有不确定的修改，需要看一下矩阵变换 ###
                            ##########################################
                            # 调用函数
                            dst_point = cvt_pos(src_point, H)
                            # apoint = cvt_pos(refPtTL[0],H)
                            # bpoint = cvt_pos(refPtTR[1],H)
                            # cpoint = cvt_pos(refPtBR[2],H)
                            # dpoint = cvt_pos(refPtBL[3],H)
                            # src_point = src_point - np.array(
                            #     [squeezed_refPtBl_3[0], squeezed_refPtBl_3[1], 1]
                            # )
                            # print("0point=" + str(refPtTL[0]))
                            # print("00point=" + str(apoint))
                            # print("1point=" + str(refPtTR[1]))
                            # print("11point=" + str(bpoint))
                            # print("3point=" + str(refPtBR[2]))
                            # print("cpoint=" + str(cpoint))
                            # print("4point=" + str( refPtBL[3]))
                            # print("dpoint=" + str(dpoint))
                            # print("dst_point="+str(dst_point))
                            # gaze_converted coordinate
                            # gazex_cvrtd = dst_point[0] / dst_point[2]
                            # gazey_cvrtd = dst_point[1] / dst_point[2]
                            # if(dst_point[0]>0 and dst_point[0]<1920 and dst_point[1]>0 and dst_point[1]<1080):
                            print("x ：", dst_point[0], "y:", dst_point[1])
                            # else:
                            #     print("out of range")
                            # 将数据转换为字符串
                            data_str = f"{ dst_point[0]},{ dst_point[1]}"
                            # 发送数据给Unity应用程序
                            unity_socket.sendall(data_str.encode())

                            # 等待一段时间，控制数据生成的速率
                            # time.sleep(0.1)

                        # elif i % 50 == 0:
                        #     logging.info(
                        #         "No gaze data received. Have you tried putting on the glasses?"
                        #     )

                    #################################################
                    ### 视频                                      ###
                    #################################################
                    # cv2.imshow("Video", frame)  # type: ignore
                    #################################################

                    # cv2.waitKey(50)  # type: ignore
                    if cv2.waitKey(1) & 0xFF == 27:
                        # unity_socket.close()
                        # server_socket.close()
                        break
                    # if 0xFF == ord("q"):  # 按下 'q' 键退出
                    #     break

                    
                    


def main():
    asyncio.run(stream_rtsp())
    cv2.destroyAllWindows()


if __name__ == "__main__":
    dotenv.load_dotenv()
    main()
