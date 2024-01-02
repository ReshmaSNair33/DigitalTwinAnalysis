# DigitalTwinAnalysis

Refer website https://sites.google.com/view/digitaltwinanavis/home for introduction

Case Study in detail
---------------------
In our study, we treat the robotic car as if it were a regular track on the road. We've designed a specific track for the robot's navigation. Our experiment involves two users: User1 and User2. User1 will navigate through the track at a constant speed, while User2's journey varies in speed, adapting to the signboards along the route. By analyzing and visualizing data from these distinct user experiences, we aim to gain valuable insights into the robotic car's performance and capabilities. 

When we implement a project using digital twin, it will have mainly three components ie Real World, Virtual World and the integration between the two worlds as below:


1) Real World:

In our setup, 'the real world' encompasses the Robomaster EP robotic car, the track through which it traverses, and the signboards it encounters. To facilitate control and interaction, we've developed a user-friendly GUI for this project. Through this interface, users can choose whether User1 or User2 should navigate. Additionally, it allows for the customization of the robot's speed. Once the user and speed are selected, simply click the 'Run' button to set the robotic car in motion.

2) Virtual World: 

Virtual World refers to the meticulous recreation of the real world within a digital environment. This process involves capturing real-world dimensions, features, and characteristics to construct an accurate virtual model. We have harnessed the power of Unity software to forge a virtual counterpart of our physical world. This breakthrough enables precise manipulation of the Robomaster in the virtual realm, mirroring its real-world movements. Furthermore, utilizing the stored data, we conduct detailed analyses and visualizations within Unity, offering insightful feedback to users. 

3) Integration between Real World and Virtual World: 

Integration is the practice of securely gathering and transmitting data to accurately mirror the real world within the virtual realm, and vice versa. To achieve this, we connect the Robomaster to a computer via Wi-Fi and establish a TCP/IP connection. The Robomaster is equipped with SDK capabilities, enabling us to employ Python programming for transmitting sensor data from the real world to Unity. Within Unity software, we utilize C# programming to acquire data from the virtual environment and relay it back to the Robomaster. 

Try It Out -- step by step procedure to perform our work using RoboMaster EP
----------------------------------------------------------------------------

Step 1 - Dive into our Digital Twin Analysis Demonstration with these easy-to-follow steps, designed for you to independently navigate the experience: 

Step 2 - Power Up: Activate the RoboMaster and establish a Wi-Fi connection to your computer. 

Step 3 - Launch the Software: Execute the command Python3 pathname/DigitalTwin.py in your terminal to start the RoboMaster program. 

Step 4 - Virtual World Access: Open the Digital Twin Project in Unity and hit the 'Play' button to enter game mode for a virtual representation. 

Step 5 - User Selection: In the User Interface, choose your user profile (e.g., User1 or User2) in either of the environments. 

Step 6 - Control Speed: Adjust the movement speed as you desire in both the real and virtual environments after User Selection. 

Step 7 - Practice and Learn: Repeat the steps to enhance your understanding of the Digital Twin concept. 

Step 8 - Feedback and Analysis: After experimenting, click on 'Feedback' to view data analysis and visualizations, such as Bar Graphs or Line Graphs, reflecting user activities. 

Step 9 - Conclude Session: Click on 'Cancel' when you are ready to end the operation. 

Step 10 - Deepen Your Understanding: For a comprehensive insight into the operation flow of the Digital Twin, consult the system architecture diagram below for additional information. 

Engage with our Digital Twin Analysis and experience the seamless integration of the virtual and real worlds! 
