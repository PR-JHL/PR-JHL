import { create } from 'zustand';

export interface BluetoothDevice {
  id: string;
  name: string;
  rssi: number;
  connected: boolean;
}

export interface ChannelData {
  timestamp: number;
  values: number[];
}

export interface Channel {
  id: number;
  name: string;
  color: string;
  visible: boolean;
  gain: number;
  offset: number;
}

export interface Command {
  id: string;
  name: string;
  payload: string;
  timestamp: number;
}

export const CHANNELS: Channel[] = [
  { id: 1, name: 'Channel 1', color: '#e74c3c', visible: true, gain: 1, offset: 0 },
  { id: 2, name: 'Channel 2', color: '#9b59b6', visible: true, gain: 1, offset: 0 },
  { id: 3, name: 'Channel 3', color: '#3498db', visible: false, gain: 1, offset: 0 },
  { id: 4, name: 'Channel 4', color: '#1abc9c', visible: false, gain: 1, offset: 0 },
  { id: 5, name: 'Channel 5', color: '#f1c40f', visible: false, gain: 1, offset: 0 },
  { id: 6, name: 'Channel 6', color: '#e67e22', visible: false, gain: 1, offset: 0 },
  { id: 7, name: 'Channel 7', color: '#fd79a8', visible: true, gain: 1, offset: 0 },
  { id: 8, name: 'Channel 8', color: '#00b894', visible: true, gain: 1, offset: 0 },
];

interface AppState {
  connectedDevice: BluetoothDevice | null;
  channelData: ChannelData[];
  channels: Channel[];
  commandHistory: Command[];
  isConnected: boolean;
  bufferSize: number;
  plotWidth: number;
  setConnectedDevice: (device: BluetoothDevice | null) => void;
  addChannelData: (data: ChannelData) => void;
  addCommand: (command: Command) => void;
  setConnected: (connected: boolean) => void;
  clearChannelData: () => void;
  toggleChannelVisibility: (channelId: number) => void;
  updateChannelGain: (channelId: number, gain: number) => void;
  updateChannelOffset: (channelId: number, offset: number) => void;
  setBufferSize: (size: number) => void;
  setPlotWidth: (width: number) => void;
  showAllChannels: () => void;
  hideAllChannels: () => void;
}

export const useAppStore = create<AppState>((set) => ({
  connectedDevice: null,
  channelData: [],
  channels: CHANNELS,
  commandHistory: [],
  isConnected: false,
  bufferSize: 1000,
  plotWidth: 1000,
  setConnectedDevice: (device) => set({ connectedDevice: device }),
  addChannelData: (data) => set((state) => ({
    channelData: [...state.channelData.slice(-(state.bufferSize - 1)), data]
  })),
  addCommand: (command) => set((state) => ({
    commandHistory: [command, ...state.commandHistory.slice(49)]
  })),
  setConnected: (connected) => set({ isConnected: connected }),
  clearChannelData: () => set({ channelData: [] }),
  toggleChannelVisibility: (channelId) => set((state) => ({
    channels: state.channels.map(ch => 
      ch.id === channelId ? { ...ch, visible: !ch.visible } : ch
    )
  })),
  updateChannelGain: (channelId, gain) => set((state) => ({
    channels: state.channels.map(ch => 
      ch.id === channelId ? { ...ch, gain } : ch
    )
  })),
  updateChannelOffset: (channelId, offset) => set((state) => ({
    channels: state.channels.map(ch => 
      ch.id === channelId ? { ...ch, offset } : ch
    )
  })),
  setBufferSize: (size) => set({ bufferSize: size }),
  setPlotWidth: (width) => set({ plotWidth: width }),
  showAllChannels: () => set((state) => ({
    channels: state.channels.map(ch => ({ ...ch, visible: true }))
  })),
  hideAllChannels: () => set((state) => ({
    channels: state.channels.map(ch => ({ ...ch, visible: false }))
  })),
}));
