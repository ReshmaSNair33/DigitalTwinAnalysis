import sys
import csv
import cv2
import time
import os
from PyQt5.QtWidgets import QApplication, QMainWindow, QPushButton, QVBoxLayout, QWidget, QLabel, QLineEdit
from robomaster import robot, vision
from PyQt5.QtCore import Qt
from PyQt5.QtGui import QFont
from datetime import datetime
from PyQt5.QtCore import QTimer
from PyQt5.QtCore import pyqtSignal
from PyQt5.QtCore import QThread

ep_robot = robot.Robot()
ep_robot.initialize(conn_type="ap")
ep_chassis = ep_robot.chassis
ep_vision = ep_robot.vision
ep_camera = ep_robot.camera



def log_session_summary(user,  average_speed, total_time):
    """Log session summary to a CSV file."""
    filename = 'session_summary.csv'
    file_exists = os.path.isfile(filename) and os.path.getsize(filename) > 0
    formatted_average_speed = round(average_speed, 2)
    formatted_total_time = round(total_time, 2)

    with open(filename, 'a', newline='') as csvfile:
        writer = csv.writer(csvfile)
        if not file_exists:
            writer.writerow(['User', 'Average_Speed', 'Total_Time'])
        writer.writerow([user,  formatted_average_speed, formatted_total_time])

def log_user_session_details(user, session_state, speed, average_speed):
    """Log user session details to a CSV file."""
    filename = f'{user}_session_details.csv'
    file_exists = os.path.isfile(filename) and os.path.getsize(filename) > 0

    with open(filename, 'a', newline='') as csvfile:
        writer = csv.writer(csvfile)
        if not file_exists:
            writer.writerow(['User', 'Session_State', 'Speed', 'Average_Speed', 'Timestamp'])
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        writer.writerow([user, session_state, speed, average_speed, timestamp])

def log_user_session(user, session_start=True):
    """Log the start or end of a user's session to a CSV file."""
    file_exists = os.path.isfile('user_sessions.csv') and os.path.getsize('user_sessions.csv') > 0

    with open('user_sessions.csv', 'a', newline='') as csvfile:
        writer = csv.writer(csvfile)
        if not file_exists:
            writer.writerow(['User', 'Timestamp', 'Session_Type'])
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        session_type = "Start" if session_start else "End"
        writer.writerow([user, timestamp, session_type])


def save_user_click_to_csv(user):
    """Save the clicked user button to a CSV file."""
    file_exists = os.path.isfile('user_clicks.csv') and os.path.getsize('user_clicks.csv') > 0

    with open('user_clicks.csv', 'a', newline='') as csvfile:
        writer = csv.writer(csvfile)
        if not file_exists:
            writer.writerow(['Button_Clicked', 'Timestamp'])
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        writer.writerow([user, timestamp])


def save_speed_to_csv(speed):
    """Save the speed input by the user to a CSV file."""
    with open('speed_input.csv', 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow(['Speed'])
        writer.writerow([speed])

def read_speed_from_csv():
    """Read the speed from the CSV file."""
    with open('speed_input.csv', 'r') as csvfile:
        reader = csv.reader(csvfile)
        next(reader)  # Skip the header
        speed = float(next(reader)[0])
    return speed


def move_robot_with_marker(main_ui, should_move=True):
    start_time = time.time()
    log_user_session("User2", session_start=True)
    if not should_move:
        main_ui.show()
        return
    speed = read_speed_from_csv()
    log_user_session_details("User2", "start", speed, 0)
    def sub_position_handler(position_info):
        x, y, z = position_info
        current_dnt = datetime.now()
        print("chassis position: x:{0}, y:{1}, z:{2},date:{3}".format(x, y, z,current_dnt))
        with open('current_robot_position.csv', 'w', newline='') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(['X_Position', 'Y_Position', 'Z_Position'])
            writer.writerow([x, y, z])
       

    ep_chassis.sub_position(freq=2, callback=sub_position_handler)

    detected_markers = set()

    def on_detect_marker(marker_info):
        for marker in marker_info:
            x, y, w, h, info = marker
            if w > 0.2:
                if info not in detected_markers:
                    print("entered if")
                    detected_markers.add(info)
                    with open('marker_positions.csv', 'a', newline='') as csvfile:
                        print("entered")
                        position_writer = csv.writer(csvfile)
                        print("info")
                        position_writer.writerow([x, y, info])
                        print("marker:{0} x:{1}, y:{2}, w:{3}, h:{4}".format(info, x, y, w, h))

    with open('marker_positions.csv', 'w', newline='') as csvfile:
        position_writer = csv.writer(csvfile)
        position_writer.writerow(['X_Position', 'Y_Position', 'Marker_Info'])

    ep_vision.sub_detect_info(name="marker", callback=on_detect_marker)

    try: 
            speed = read_speed_from_csv()    
            # t= 2/speed♣
            # t1= 0.7/speed♣
            if(speed == 0.5 or speed == 0.6):
                t= 1.8/speed
                # t1= 0.5/speed
                ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
                time.sleep(t)
                ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
                time.sleep(1)
            elif(speed == 0.7 or speed == 0.8):
                t= 1.85/speed
                # t1= 0.5/speed
                ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
                time.sleep(t)
                ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
                time.sleep(1)
            elif(speed ==0.9 or speed ==1):
                t= 2.1/speed
                # t1= 0.5/speed
                ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
                time.sleep(t)
                ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
                time.sleep(1)
            t= 1.3/speed    
            t1= 0.4/speed
            ep_chassis.drive_speed(x=1.0, y=0, z=0, timeout=t1)
            time.sleep(t1)
            log_user_session_details("User2", "ongoing", 0.8, 0)

            ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
            time.sleep(1)

            t= 1.10/speed
            t1= 0.1/speed
            ep_chassis.drive_speed(x=1.0, y=0, z=0, timeout=t)
            time.sleep(t)
            
            ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
            time.sleep(1)
            log_user_session_details("User2", "ongoing", 1, 0)

            t= 1.3/speed
            t1= 0.8/speed
            ep_chassis.drive_speed(x=0.6, y=0, z=0, timeout=t1)
            time.sleep(t1)
            ep_chassis.drive_speed(x=0.5, y=0, z=90, timeout=1)
            time.sleep(1)
            log_user_session_details("User2", "ongoing", 0.6, 0)
            speeds = [speed, 0.8, 1.0,0.6]  # Add all the speeds used in the session
            average_speed = sum(speeds) / len(speeds)
           
            log_user_session_details("User2", "end", average_speed, average_speed)

            log_user_session("User2", session_start=False)
            total_time = time.time() - start_time 
            log_session_summary("User2", average_speed, total_time)
            main_ui.robotMovementCompleted.emit()

            

    finally:
        # log_user_session("User2", session_start=False)
        header = ['X_Position', 'Y_Position', 'Z_Position']
        data= [0,0,0]
        with open("current_robot_position.csv", 'w', encoding='UTF8', newline='') as f:
                writer = csv.writer(f)
                writer.writerow(header)
                writer.writerow(data)
        ep_chassis.unsub_position()
        ep_chassis.drive_speed(x=0, y=0, z=0)

def moveRobotInRectangle(main_ui,should_move=True):
    start_time = time.time() 
    log_user_session("User1", session_start=True)
    if not should_move:
        main_ui.show()
        return
    speed = read_speed_from_csv()
    log_user_session_details("User1", "start", speed, 0)
    def sub_position_handler(position_info):
        x, y, z = position_info
        # current_dnt = datetime.now()
        print("chassis position: x:{0}, y:{1}, z:{2}".format(x, y, z))
        with open('current_robot_position.csv', 'w', newline='') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(['X_Position', 'Y_Position', 'Z_Position'])
            writer.writerow([x, y, z])
        
    ep_chassis.sub_position(cs=0, freq=10, callback=sub_position_handler)

   
    try:
        speed = read_speed_from_csv()    
        if(speed == 0.5 or speed == 0.6):
           
            t= 1.8/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.8/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t= 1.6/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.8/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)


        elif(speed == 0.7 or speed ==0.8):

            t= 1.8/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.7/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t= 1.6/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.65/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
        
        elif(speed == 0.9 or speed == 1):

            t= 2.1/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.55/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t= 1.9/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t)
            time.sleep(t)

            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
            t1= 0.55/speed
            ep_chassis.drive_speed(x=speed, y=0, z=0, timeout=t1)
            time.sleep(t1)
    
            ep_chassis.drive_speed(x=speed, y=0, z=90, timeout=1)
            time.sleep(1)
        

        final_speed = speed
        log_user_session_details("User1", "end", final_speed, final_speed)  # Average speed is the same as initial speed
        log_user_session("User1", session_start=False)
        total_time = time.time() - start_time  # Calculate total time
        final_speed = speed
        log_session_summary("User1", final_speed, total_time)
        main_ui.robotMovementCompleted.emit()

    finally:
           
            header = ['X_Position', 'Y_Position', 'Z_Position']
            data= [0,0,0]
            with open("current_robot_position.csv", 'w', encoding='UTF8', newline='') as f:
                writer = csv.writer(f)
                writer.writerow(header)
                writer.writerow(data)
            ep_chassis.unsub_position()
            ep_chassis.drive_speed(x=0, y=0, z=0)

class RobotMovementThread(QThread):
    def __init__(self, main_ui, user_action):
        super().__init__()
        self.main_ui = main_ui
        self.user_action = user_action

    def run(self):
        if self.user_action == "User1":
            moveRobotInRectangle(self.main_ui)
        elif self.user_action == "User2":
            move_robot_with_marker(self.main_ui)
        self.main_ui.robotMovementCompleted.emit()

class MainUI(QMainWindow):
    robotMovementCompleted = pyqtSignal()
    def __init__(self):
        super().__init__()
        self.initUI()
        self.user_window_open = False
        self.robotMovementCompleted.connect(self.onRobotMovementCompleted)
        QTimer.singleShot(5000, self.check_user_clicks_csv)
    
    def onRobotMovementCompleted(self):
        self.show()

    def check_user_clicks_csv(self):
        if self.user_window_open:
            return
        try:
            with open('user_clicks.csv', 'r') as csvfile:
                reader = csv.reader(csvfile)
                last_row = None
                for row in reader:
                    last_row = row
                if last_row and last_row[0] == "User1":
                    self.user_window_open = True
                    self.hide()
                    moveRobotInRectangle(self)
                    self.reset_user_clicks_csv()  # Reset the command in CSV
                    self.show()
                    self.user_window_open = False
                elif last_row and last_row[0] == "User2":
                    self.user_window_open = True
                    self.hide()
                    move_robot_with_marker(self)
                    self.reset_user_clicks_csv()  # Reset the command in CSV
                    self.show()
                    self.user_window_open = False
        except FileNotFoundError:
            pass
        QTimer.singleShot(1000, self.check_user_clicks_csv)

    def reset_user_clicks_csv(self):
        """Reset the command in the user_clicks.csv file."""
        with open('user_clicks.csv', 'w', newline='') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(['Button_Clicked', 'Timestamp'])  # Write the header again
    

    def initUI(self):
        self.setWindowTitle('RoboMaster Control Panel')
        self.resize(600, 400)
        QTimer.singleShot(5000, self.check_user_clicks_csv) 

        # Create buttons
        self.user1_btn = QPushButton('User1', self)
        self.user1_btn.setFixedSize(200, 50) 
        self.user1_btn.clicked.connect(self.openUserWindowForRectangle)

        self.user2_btn = QPushButton('User2', self)
        self.user2_btn.setFixedSize(200, 50) 
        self.user2_btn.clicked.connect(self.openUserWindowForMarker)

        self.cancel_btn = QPushButton('Cancel', self)
        self.cancel_btn.setFixedSize(200, 50) 
        self.cancel_btn.clicked.connect(self.close)
        # self.check_user_clicks_csv()

        # Layout
        layout = QVBoxLayout()
        layout.addStretch()  # Add a stretchable space before the buttons
        layout.addWidget(self.user1_btn, 0, Qt.AlignCenter)  # Center the button
        layout.addWidget(self.user2_btn, 0, Qt.AlignCenter)  # Center the button
        layout.addWidget(self.cancel_btn, 0, Qt.AlignCenter)  # Center the button
        layout.addStretch()
        

        central_widget = QWidget()
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)

   

    def openUserWindowForRectangle(self):
        save_user_click_to_csv("User1")
        self.user_window = UserWindow(self, "User1")
        self.robotMovementCompleted.emit()
        font_label = QFont()
        font_label.setPointSize(14)
        self.user_window.show()
        self.hide()
        self.user_window_open = True   

    def openUserWindowForMarker(self):
        save_user_click_to_csv("User2")
        self.user_window = UserWindow(self, "User2")
        font_label = QFont()
        font_label.setPointSize(14)
        self.user_window.show()
        self.hide()
        self.user_window_open = True

   

    def closeEvent(self, event):
        save_user_click_to_csv("Cancel")
        self.user_window_open = False  # Reset the flag when MainUI is closed
        super().closeEvent(event)
        ep_robot.close()

    def runRobot(self):
        speed = float(self.speed_input.text())
        save_speed_to_csv(speed)
        self.close()  # Close the UserWindow
        self.worker_thread = RobotMovementThread(self.main_window, self.user_action)
        self.worker_thread.start()
    

class UserWindow(QMainWindow):
    def __init__(self,  main_window,user_action):
        super().__init__()
        self.main_window = main_window
        self.user_action = user_action
        self.initUI()

    def initUI(self):
        self.setWindowTitle('Set Initial Speed')
        self.resize(600, 400)
      

        self.label = QLabel('Enter Speed [0.5-1 m/s]:', self)
        font_label = QFont()
        font_label.setPointSize(14)  # Adjust the size as needed
        self.label.setFont(font_label)

        
        self.speed_input = QLineEdit(self)

        font_input = QFont()
        font_input.setPointSize(14)  # Adjust the size as needed
        self.speed_input.setFont(font_input)
        self.speed_input.setFixedHeight(40)
        
        self.run_btn = QPushButton('Run', self)
        self.run_btn.setFixedSize(150, 50)
        self.run_btn.clicked.connect(self.runRobot)

        # Layout
        layout = QVBoxLayout()
        layout.addStretch()
        layout.addWidget(self.label, 0, Qt.AlignCenter)
        layout.addWidget(self.speed_input, 0, Qt.AlignCenter)
        layout.addWidget(self.run_btn, 0, Qt.AlignCenter)
        layout.addStretch()
       
        central_widget = QWidget()
        central_widget.setLayout(layout)
        self.setCentralWidget(central_widget)

    def runRobot(self):
        speed = float(self.speed_input.text())
        save_speed_to_csv(speed)
        self.close()  # Close the UserWindow
        if self.user_action == "User1":
            moveRobotInRectangle(self.main_window)
        elif self.user_action == "User2":
            move_robot_with_marker(self.main_window)
        self.main_window.robotMovementCompleted.emit()  # Emit the 




if __name__ == '__main__':
    app = QApplication(sys.argv)
    main_ui = MainUI()
    main_ui.show()
    sys.exit(app.exec_())