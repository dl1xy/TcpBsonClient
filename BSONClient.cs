﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.2012
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Kernys.Bson;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TcpBsonClient;
using System.Text;

public class BSONClient : MonoBehaviour
{
	//private TcpClient client;
	private const Int32 PORT = 9999;
	private const string SERVER_ADDR = "192.168.1.1";
	private CmdManager cmdMgr;
	
	internal Boolean socket_ready = false;
	internal String input_buffer = "";
	private TcpClient tcp_socket;
	private NetworkStream net_stream;
	
	private BinaryWriter socket_writer;
	private IObserver observer;
	


	void Update()
	{
		readSocket();
		writeSocket ();		
	}
	


	void Awake()
	{
		this.cmdMgr = new CmdManager ();
		setupSocket();
	}
	
	void OnApplicationQuit()
	{
		closeSocket();
	}

	public void addObserver (IObserver observer)
	{
		this.observer = observer;
	}
	
	private void setupSocket()
	{
		try
		{
			tcp_socket = new TcpClient();
			tcp_socket.Connect(IPAddress.Parse(SERVER_ADDR), PORT);
			net_stream = tcp_socket.GetStream();
			socket_writer = new BinaryWriter(net_stream);
					
			socket_ready = true;
		}
		catch (Exception e)
		{
			// Something went wrong
			Debug.Log("Socket error: " + e);
		}
	}

	
	public void closeSocket()
	{
		if (!socket_ready)
			return;
		
		socket_writer.Close();
		tcp_socket.Close();
		socket_ready = false;
	}
	
	public void addServerCmd(IServerCmd cmd)
	{
		cmdMgr.addServerCmd (cmd);
	}
	
	
	
	private void readSocket()
	{
		//Console.WriteLine(" - Try to read!");
		if (!socket_ready)
			return;

		if (net_stream.DataAvailable && net_stream.CanRead) {
			//Debug.Log("Read Socket");	
			try {
				// decrypt ? 
				BSONObject obj = null;
				obj = SimpleBSON.Load( ByteStreamParser.parseStream(net_stream));



				IClientCmd clientCmd = cmdMgr.decodeBSON(obj);
				if (clientCmd != null)
				{
					this.observer.addClientCmd (clientCmd);
				}
				else
				{
					Debug.LogError("ClientCmd not parsed !");
				}


			} catch (Exception e) {
				Debug.LogError ("Exception:"+ e);
			}
		}
	}
	
	private void writeSocket()
	{
		
		if (!socket_ready)
			return;
		
		Queue<IServerCmd> srvCmdList = this.cmdMgr.getServerCmds ();
		if (srvCmdList.Count <= 0)
			return;
		//Debug.Log("Write Socket");	
		for (var i=0; i < srvCmdList.Count; i++) 
		{
			IServerCmd cmd = srvCmdList.Dequeue();
			BSONObject obj = cmd.encode();
			byte[] raw = SimpleBSON.Dump(obj);

			socket_writer.Write(raw, 0, raw.Length);

			socket_writer.Flush();
			//Debug.Log (" - Command written!");
		}
	}
}
 
